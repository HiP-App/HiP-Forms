using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    class Quiz:IIdentifiable
    {
        public string Status { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Text { get;  set; }

        public int Id { get;  set; }

        public Exhibit Exhibit { get; set; }

        public string[] Options { get;  set; }
 
        public Image Image { get;  set; }
        string IIdentifiable.Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
