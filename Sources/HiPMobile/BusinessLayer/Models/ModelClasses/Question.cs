using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    class Question
    {
        public string Status { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public string Text { get; private set; }

        public int Id { get; private set; }

        public Exhibit Exhibit { get; private set; }

        public string[] Options { get; private set; }
 
        public Image Image { get; private set; }

    }
}
