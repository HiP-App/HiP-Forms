// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    public class UserRating
    {
        public UserRating(UserRatingDto userRatingDto)
        {
            Average = userRatingDto.Average;
            Count = userRatingDto.Count;

            NumOneStarRatings = userRatingDto.RatingTable.TryGetValue(1, out var n1) ? n1 : 0;
            NumTwoStarRatings = userRatingDto.RatingTable.TryGetValue(2, out var n2) ? n2 : 0;
            NumThreeStarRatings = userRatingDto.RatingTable.TryGetValue(3, out var n3) ? n3 : 0;
            NumFourStarRatings = userRatingDto.RatingTable.TryGetValue(4, out var n4) ? n4 : 0;
            NumFiveStarRatings = userRatingDto.RatingTable.TryGetValue(5, out var n5) ? n5 : 0;
        }

        public UserRating(int numOneStarRatings, int numTwoStarRatings, int numThreeStarRatings, int numFourStarRatings, int numFiveStarRatings)
        {
            Count = numOneStarRatings + numTwoStarRatings + numThreeStarRatings + numFourStarRatings + numFiveStarRatings;
            var starSum = 1 * numOneStarRatings + 2 * numTwoStarRatings + 3 * numThreeStarRatings + 4 * numFourStarRatings + 5 * numFiveStarRatings;
            Average = Count == 0 ? 0 : (double) starSum / Count;

            NumOneStarRatings = numOneStarRatings;
            NumTwoStarRatings = numTwoStarRatings;
            NumThreeStarRatings = numThreeStarRatings;
            NumFourStarRatings = numFourStarRatings;
            NumFiveStarRatings = numFiveStarRatings;
        }

        public double Average { get; }

        public int Count { get; }

        public int NumOneStarRatings { get; }

        public int NumTwoStarRatings { get; }

        public int NumThreeStarRatings { get; }

        public int NumFourStarRatings { get; }

        public int NumFiveStarRatings { get; }
    }
}