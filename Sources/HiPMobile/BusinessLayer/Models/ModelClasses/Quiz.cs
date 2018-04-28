using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public class Quiz : IIdentifiable
    {
        public string Status { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Text { get; set; }

        public string Id { get; set; }

        public Exhibit Exhibit { get; set; }

        /**
         * By convention, the first option is always the correct one.
         */
        [NotMapped]
        public string[] Options
        {
            get => new[] { OptionA, OptionB, OptionC, OptionD };

            set
            {
                Debug.Assert(value.Length == 4, "A quiz must have exactly 4 options.");
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