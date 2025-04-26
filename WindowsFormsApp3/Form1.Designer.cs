using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class MiniCalendar : Form
{
    private MonthCalendar calendar = new MonthCalendar();
    private TextBox eventInput = new TextBox();
    private DateTimePicker timePicker = new DateTimePicker();
    private Button addBtn = new Button();
    private List<(DateTime date, string desc)> events = new List<(DateTime, string)>();
    private Timer reminderTimer = new Timer();

    public MiniCalendar()
    {
        // Настройка формы
        Text = "Мини-Календарь";
        Size = new Size(400, 300);

        // Ручная реализация Placeholder
        eventInput.Text = "Событие";
        eventInput.ForeColor = Color.Gray;
        eventInput.GotFocus += (s, e) => {
            if (eventInput.Text == "Событие")
            {
                eventInput.Text = "";
                eventInput.ForeColor = Color.Black;
            }
        };
        eventInput.LostFocus += (s, e) => {
            if (string.IsNullOrEmpty(eventInput.Text))
            {
                eventInput.Text = "Событие";
                eventInput.ForeColor = Color.Gray;
            }
        };

        // Расположение элементов
        calendar.Location = new Point(10, 10);
        eventInput.Location = new Point(10, 180);
        eventInput.Width = 150;

        timePicker.Format = DateTimePickerFormat.Time;
        timePicker.ShowUpDown = true;
        timePicker.Location = new Point(170, 180);
        timePicker.Width = 80;

        addBtn.Text = "Добавить";
        addBtn.Location = new Point(260, 180);
        addBtn.Click += (s, e) => AddEvent();

        // Таймер напоминаний
        reminderTimer.Interval = 60000;
        reminderTimer.Tick += CheckReminders;
        reminderTimer.Start();

        Controls.AddRange(new Control[] { calendar, eventInput, timePicker, addBtn });
    }

    void AddEvent()
    {
        if (string.IsNullOrWhiteSpace(eventInput.Text) || eventInput.Text == "Событие") return;

        var eventDate = calendar.SelectionStart.Date.Add(timePicker.Value.TimeOfDay);
        events.Add((eventDate, eventInput.Text));
        eventInput.Clear();
        MessageBox.Show($"Событие добавлено на {eventDate:g}");
    }

    void CheckReminders(object sender, EventArgs e)
    {
        var now = DateTime.Now;
        foreach (var ev in events)
        {
            if (Math.Abs((ev.date - now).TotalMinutes) < 1)
                MessageBox.Show($"Напоминание: {ev.desc}", "Время пришло!");
        }
    }

    [STAThread]
    static void Main() => Application.Run(new MiniCalendar());
}
