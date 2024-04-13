using System.Text.Json.Serialization;

namespace CountdownSticker.Models
{
    public class Countdown
    {
        private Guid _id;
        private string _title;
        private string? _note;
        private DateTime _endTime;
        private DateTime _lastModified;
        private bool _isVisible;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string? Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }

        [JsonIgnore]
        public TimeSpan Remaining
        {
            get { return IsActive ? _endTime - DateTime.Now : TimeSpan.Zero; }
        }

        [JsonIgnore]
        public bool IsActive
        {
            get { return _endTime >= DateTime.Now; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public Countdown()
        {
            _id = Guid.NewGuid();
            _title = "新的倒计时";
            _note = string.Empty;
            _isVisible = true;
            _endTime = DateTime.Today.AddDays(1).AddHours(9);
            _lastModified = DateTime.Now;
        }
    }
}
