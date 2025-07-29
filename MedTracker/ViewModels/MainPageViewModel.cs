using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MedTracker.Application.Interfaces;
using MedTracker.Core.Models;

namespace MedTracker.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IMedicationLogService _medicationLogService;
        private readonly IScheduleService _scheduleService;
        private bool _isLoading;
        private string _welcomeMessage;
        private int _todayMedicationsCount;
        private int _takenMedicationsCount;

        public MainPageViewModel(IMedicationLogService medicationLogService, IScheduleService scheduleService)
        {
            _medicationLogService = medicationLogService;
            _scheduleService = scheduleService;

            TodaySchedule = new ObservableCollection<DailyScheduleItem>();
            UpcomingSchedule = new ObservableCollection<DailyScheduleItem>();

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            MarkAsTakenCommand = new Command<DailyScheduleItem>(async (item) => await MarkAsTakenAsync(item));
            RefreshCommand = new Command(async () => await RefreshAsync());

            SetWelcomeMessage();
            _ = LoadDataAsync();
        }

        #region Properties

        public ObservableCollection<DailyScheduleItem> TodaySchedule { get; }
        public ObservableCollection<DailyScheduleItem> UpcomingSchedule { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        public int TodayMedicationsCount
        {
            get => _todayMedicationsCount;
            set => SetProperty(ref _todayMedicationsCount, value);
        }

        public int TakenMedicationsCount
        {
            get => _takenMedicationsCount;
            set => SetProperty(ref _takenMedicationsCount, value);
        }

        public double CompletionPercentage => TodayMedicationsCount > 0
            ? (double)TakenMedicationsCount / TodayMedicationsCount * 100
            : 0;

        public string ProgressText => $"{TakenMedicationsCount} из {TodayMedicationsCount}";

        #endregion

        #region Commands

        public ICommand LoadDataCommand { get; }
        public ICommand MarkAsTakenCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Private Methods

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Загружаем расписание на сегодня
                var todaySchedule = await _scheduleService.GetDailyScheduleAsync(DateTime.Today);
                var scheduleWithLogs = await _medicationLogService.UpdateScheduleWithLogsAsync(todaySchedule);

                TodaySchedule.Clear();
                foreach (var item in scheduleWithLogs)
                {
                    TodaySchedule.Add(item);
                }

                // Загружаем расписание на ближайшие дни (исключая сегодня)
                var upcomingSchedule = await _scheduleService.GetUpcomingScheduleAsync(DateTime.Today.AddDays(1), 3);
                var upcomingWithLogs = await _medicationLogService.UpdateScheduleWithLogsAsync(upcomingSchedule);

                UpcomingSchedule.Clear();
                foreach (var item in upcomingWithLogs.Take(6)) // Показываем только первые 6 предстоящих приемов
                {
                    UpcomingSchedule.Add(item);
                }

                UpdateStatistics();
            }
            catch (Exception ex)
            {
                // В реальном приложении здесь должна быть обработка ошибок
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task MarkAsTakenAsync(DailyScheduleItem item)
        {
            try
            {
                if (item.IsTaken) return;

                var request = new MedicationTakeRequest
                {
                    MedicationId = item.MedicationId,
                    TakenAt = DateTime.Now,
                    ConfirmedByUser = true
                };

                await _medicationLogService.RecordMedicationTakenAsync(request);

                item.IsTaken = true;
                item.TakenAt = DateTime.Now;

                UpdateStatistics();
                OnPropertyChanged(nameof(CompletionPercentage));
                OnPropertyChanged(nameof(ProgressText));
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                System.Diagnostics.Debug.WriteLine($"Ошибка отметки приема: {ex.Message}");
            }
        }

        private async Task RefreshAsync()
        {
            await LoadDataAsync();
        }

        private void UpdateStatistics()
        {
            TodayMedicationsCount = TodaySchedule.Count;
            TakenMedicationsCount = TodaySchedule.Count(item => item.IsTaken);

            OnPropertyChanged(nameof(CompletionPercentage));
            OnPropertyChanged(nameof(ProgressText));
        }

        private void SetWelcomeMessage()
        {
            var hour = DateTime.Now.Hour;
            WelcomeMessage = hour switch
            {
                >= 5 and < 12 => "Доброе утро!",
                >= 12 and < 17 => "Добрый день!",
                >= 17 and < 22 => "Добрый вечер!",
                _ => "Доброй ночи!"
            };
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}