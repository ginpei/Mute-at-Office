using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Mute_at_Office.Libs.Agent;
using System;

namespace Mute_at_Office.Pages.History
{
    public sealed partial class HistoryItemControl : UserControl
    {
        public static readonly DependencyProperty DateTimeTextProperty =
            DependencyProperty.Register(
                nameof(DateTimeText),
                typeof(object),
                typeof(HistoryItemControl),
                new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register(
                nameof(EventType),
                typeof(object),
                typeof(HistoryItemControl),
                new PropertyMetadata(null, OnPropertyChanged));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(HistoryItemControl),
                new PropertyMetadata(string.Empty, OnPropertyChanged));

        public HistoryItemControl()
        {
            InitializeComponent();
            Loaded += HistoryItemControl_Loaded;
            Unloaded += HistoryItemControl_Unloaded;
        }

        public object DateTimeText
        {
            get => GetValue(DateTimeTextProperty);
            set => SetValue(DateTimeTextProperty, value);
        }

        public object EventType
        {
            get => GetValue(EventTypeProperty);
            set => SetValue(EventTypeProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        private void HistoryItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }

        private void HistoryItemControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= HistoryItemControl_Loaded;
            Unloaded -= HistoryItemControl_Unloaded;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HistoryItemControl control && control.IsLoaded)
            {
                control.UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (HistoryTextBlock == null)
            {
                return;
            }

            HistoryTextBlock.Inlines.Clear();
            
            var dateTimeStr = DateTimeText switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                string s => s,
                _ => DateTimeText?.ToString() ?? ""
            };
            
            var eventTypeStr = EventType switch
            {
                LookoutEventType et => et switch
                {
                    LookoutEventType.MuteAtOffice => "Mute-at-Office",
                    LookoutEventType.WiFi => "WiFi",
                    LookoutEventType.Audio => "Audio",
                    _ => et.ToString()
                },
                string s => s,
                _ => EventType?.ToString() ?? ""
            };
            
            var dateTimeRun = new Run
            {
                Text = $"{dateTimeStr} - [{eventTypeStr}] ",
                FontSize = 10,
                FontWeight = Microsoft.UI.Text.FontWeights.Light
            };
            var messageRun = new Run { Text = Message };
            
            HistoryTextBlock.Inlines.Add(dateTimeRun);
            HistoryTextBlock.Inlines.Add(messageRun);
        }
    }
}
