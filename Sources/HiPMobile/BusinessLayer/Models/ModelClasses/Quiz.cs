using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

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
        public IReadOnlyList<string> Options
        {
            get => new[] { OptionA, OptionB, OptionC, OptionD };

            set
            {
                Debug2.Assert(value.Count == 4, "A quiz must have exactly 4 options.");
                OptionA = value[0];
                OptionB = value[1];
                OptionC = value[2];
                OptionD = value[3];
            }
        }

        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        public Image Image { get; set; }
    }
}