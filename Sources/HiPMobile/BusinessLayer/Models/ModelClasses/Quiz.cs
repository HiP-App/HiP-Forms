using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public class Quiz : IIdentifiable
    {
        public string Status { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Text { get; set; }

        public string Id { get; set; }

        public Exhibit Exhibit { get; set; }

        [NotMapped]
        public string[] Options
        {
            get => new[] { OptionA, OptionB, OptionC, OptionD };

            set
            {
                OptionA = value[0];
                OptionB = value[1];
                OptionC = value[2];
                OptionD = value[3];
            }
        }

        string OptionA { get; set; }
        string OptionB { get; set; }
        string OptionC { get; set; }
        string OptionD { get; set; }

        public Image Image { get; set; }
    }
}
