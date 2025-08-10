using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfApp1
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private Part _currentPart;
        private string _commandLog;
        private string _commandInput;
        private readonly Random _random = new Random();
        #endregion

        #region Properties for Binding
        // -- Inspection Properties
        public Part CurrentPart
        {
            get => _currentPart;
            set => SetProperty(ref _currentPart, value);
        }
        public ObservableCollection<Part> ClassifiedParts { get; set; }

        // -- Classification Criteria Properties
        public double MinLength { get; set; } = 9.9;
        public double MaxLength { get; set; } = 10.1;
        public double MinWeight { get; set; } = 4.9;
        public double MaxWeight { get; set; } = 5.1;

        // -- Command Interface Properties
        public string CommandLog
        {
            get => _commandLog;
            set => SetProperty(ref _commandLog, value);
        }
        public string CommandInput
        {
            get => _commandInput;
            set => SetProperty(ref _commandInput, value);
        }
        #endregion

        #region Commands
        public ICommand InspectNextPartCommand { get; }
        public ICommand ExecuteUserCommand { get; }
        #endregion

        public MainViewModel()
        {
            ClassifiedParts = new ObservableCollection<Part>();
            InspectNextPartCommand = new RelayCommand(InspectNextPart);
            ExecuteUserCommand = new RelayCommand(ProcessUserCommand, CanProcessUserCommand);

            InitializeCommandLog();
            InspectNextPart(null); // Load the first part
        }

        private void InspectNextPart(object obj)
        {
            var newPart = new Part
            {
                PartId = $"Part-{DateTime.Now.Ticks}",
                Length = 9.5 + _random.NextDouble(), // 9.5 to 10.5
                Weight = 4.5 + _random.NextDouble()  // 4.5 to 5.5
            };

            // Use properties for classification instead of hardcoded values
            bool isLengthOk = newPart.Length >= MinLength && newPart.Length <= MaxLength;
            bool isWeightOk = newPart.Weight >= MinWeight && newPart.Weight <= MaxWeight;

            newPart.Status = isLengthOk && isWeightOk ? "정상" : "불량";

            CurrentPart = newPart;
            ClassifiedParts.Insert(0, newPart);
        }

        #region Command Processing Logic
        private bool CanProcessUserCommand(object obj)
        {
            return !string.IsNullOrWhiteSpace(CommandInput);
        }

        private void ProcessUserCommand(object obj)
        {
            string command = CommandInput.Trim();
            LogCommand($"> {command}");

            var parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string commandType = parts[0].ToLower();

            switch (commandType)
            {
                case "상태":
                case "status":
                    ShowStatus();
                    break;
                case "기준":
                case "criteria":
                    ProcessCriteriaCommand(parts);
                    break;
                case "초기화":
                case "reset":
                    ClassifiedParts.Clear();
                    LogCommand("분류 기록을 초기화했습니다.");
                    break;
                case "도움말":
                case "help":
                    ShowHelp();
                    break;
                default:
                    LogCommand("알 수 없는 명령어입니다. '도움말'을 입력해보세요.");
                    break;
            }

            CommandInput = string.Empty;
        }

        private void ProcessCriteriaCommand(string[] parts)
        {
            if (parts.Length == 4)
            {
                string target = parts[1].ToLower();
                if (double.TryParse(parts[2], out double min) && double.TryParse(parts[3], out double max))
                {
                    if (target == "길이" || target == "length")
                    {
                        MinLength = min;
                        MaxLength = max;
                        LogCommand($"길이 기준이 {MinLength} ~ {MaxLength} (으)로 변경되었습니다.");
                        OnPropertyChanged(nameof(MinLength));
                        OnPropertyChanged(nameof(MaxLength));
                    }
                    else if (target == "무게" || target == "weight")
                    {
                        MinWeight = min;
                        MaxWeight = max;
                        LogCommand($"무게 기준이 {MinWeight} ~ {MaxWeight} (으)로 변경되었습니다.");
                        OnPropertyChanged(nameof(MinWeight));
                        OnPropertyChanged(nameof(MaxWeight));
                    }
                    else
                    {
                        LogCommand("잘못된 기준 대상입니다. '길이' 또는 '무게'를 사용하세요.");
                    }
                }
                else
                {
                    LogCommand("숫자 변환에 실패했습니다. 값을 확인해주세요.");
                }
            }
            else
            {
                LogCommand("잘못된 형식입니다. 예: 기준 [길이/무게] [최소값] [최대값]");
            }
        }

        private void ShowStatus()
        {
            int total = ClassifiedParts.Count;
            if (total == 0)
            {
                LogCommand("아직 검사된 부품이 없습니다.");
                return;
            }
            int defectiveCount = ClassifiedParts.Count(p => p.Status == "불량");
            double defectRate = (double)defectiveCount / total * 100;

            var statusReport = new StringBuilder();
            statusReport.AppendLine("--- 진행 상황 보고 ---");
            statusReport.AppendLine($"총 검사 수: {total}");
            statusReport.AppendLine($"정상 부품 수: {total - defectiveCount}");
            statusReport.AppendLine($"불량 부품 수: {defectiveCount}");
            statusReport.AppendLine($"불량률: {defectRate:F2}%");
            statusReport.Append("----------------------");
            LogCommand(statusReport.ToString());
        }

        private void ShowHelp()
        {
            var helpText = new StringBuilder();
            helpText.AppendLine("--- 사용 가능한 명령어 ---");
            helpText.AppendLine("▶ 상태 : 현재까지의 검사 통계를 봅니다.");
            helpText.AppendLine("▶ 기준 [길이/무게] [최소] [최대] : 정상 기준을 변경합니다.");
            helpText.AppendLine("    (예: 기준 길이 9.8 10.2)");
            helpText.AppendLine("▶ 초기화 : 모든 분류 기록을 삭제합니다.");
            helpText.AppendLine("▶ 도움말 : 이 도움말을 다시 봅니다.");
            helpText.Append("-------------------------");
            LogCommand(helpText.ToString());
        }

        #endregion

        #region Helper Methods
        private void InitializeCommandLog()
        {
            CommandLog = string.Empty;
            LogCommand("명령어 인터페이스가 준비되었습니다.");
            LogCommand("'도움말'을 입력하여 사용법을 확인하세요.");
        }

        private void LogCommand(string message)
        {
            CommandLog += message + "\n";
        }

        // Helper for setting property and raising event
        protected bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}