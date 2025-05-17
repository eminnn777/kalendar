using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

public class MiniCalendar : Form
{
    private MonthCalendar calendar = new MonthCalendar();
    private TextBox eventInput = new TextBox();
    private DateTimePicker timePicker = new DateTimePicker();
    private Button addBtn = new Button();
    private Button editBtn = new Button();
    private List<(DateTime date, string desc)> events = new List<(DateTime, string)>();
    private Timer reminderTimer = new Timer();

    public MiniCalendar()
    {
        // Настройка формы
        Text = "Мини-Календарь";
        Size = new Size(400, 400);

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
        calendar.DateSelected += (s, e) => DisplayEventsForSelectedDate();
        eventInput.Location = new Point(10, 220);
        eventInput.Width = 150;

        timePicker.Format = DateTimePickerFormat.Time;
        timePicker.ShowUpDown = true;
        timePicker.Location = new Point(170, 220);
        timePicker.Width = 80;

        addBtn.Text = "Добавить";
        addBtn.Location = new Point(260, 220);
        addBtn.Click += (s, e) => AddEvent();

        editBtn.Text = "Редактировать";
        editBtn.Location = new Point(260, 250);
        editBtn.Click += (s, e) => EditEvent();

        // Таймер напоминаний
        reminderTimer.Interval = 60000;
        reminderTimer.Tick += CheckReminders;
        reminderTimer.Start();

        Controls.AddRange(new Control[] { calendar, eventInput, timePicker, addBtn, editBtn });
        LoadEvents();
        UpdateCalendarColors();
    }

    void AddEvent()
    {
        if (string.IsNullOrWhiteSpace(eventInput.Text) || eventInput.Text == "Событие") return;

        var eventDate = calendar.SelectionStart.Date.Add(timePicker.Value.TimeOfDay);
        events.Add((eventDate, eventInput.Text));
        eventInput.Clear();
        MessageBox.Show($"Событие добавлено на {eventDate:g}");
        SaveEvents();
        UpdateCalendarColors();
    }

    void EditEvent()
    {
        var selectedDate = calendar.SelectionStart.Date;
        var eventToEdit = events.Find(ev => ev.date.Date == selectedDate);
        if (eventToEdit != default)
        {
            eventInput.Text = eventToEdit.desc;
            timePicker.Value = eventToEdit.date.Date.Add(eventToEdit.date.TimeOfDay);
            events.Remove(eventToEdit);
            MessageBox.Show("Событие загружено для редактирования.");
        }
        else
        {
            MessageBox.Show("Нет событий для редактирования на выбранную дату.");
        }
    }

    void DisplayEventsForSelectedDate()
    {
        var selectedDate = calendar.SelectionStart.Date;
        var eventsForDate = events.FindAll(ev => ev.date.Date == selectedDate);
        string eventList = "События на " + selectedDate.ToShortDateString() + ":\n";
        foreach (var ev in eventsForDate)
        {
            eventList += $"{ev.date.ToShortTimeString()} - {ev.desc}\n";
        }
        MessageBox.Show(eventList.TrimEnd('\n'), "События");
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

    void LoadEvents()
    {
        if (File.Exists("events.txt"))
        {
            var lines = File.ReadAllLines("events.txt");
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2 && DateTime.TryParse(parts[0], out var date))
                {
                    events.Add((date, parts[1]));
                }
            }
        }
    }

    void SaveEvents()
    {
        using (var writer = new StreamWriter("events.txt"))
        {
            foreach (var ev in events)
            {
                writer.WriteLine($"{ev.date}|{ev.desc}");
            }
        }
    }

    void UpdateCalendarColors()
    {
        foreach (var day in calendar.BoldedDates)
        {
            calendar.RemoveBoldedDate(day);
        }

        foreach (var ev in events)
        {
            calendar.AddBoldedDate(ev.date.Date);
        }

        calendar.UpdateBoldedDates();
    }

    [STAThread]
    static void Main() => Application.Run(new MiniCalendar());
}
