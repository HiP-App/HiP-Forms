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

using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.BusinessLayer.DtoToModelConverters
{
    [TestFixture]
    public class AchievementConverterTest
    {
        public AchievementConverterTest()
        {
            IoCManager.Clear();
            var sub = Substitute.For<IDataAccess>();
            IoCManager.RegisterInstance(typeof(IDataAccess), sub);
        }

        [Test, Category("UnitTest")]
        public void Test_RouteFinishedAchievementConversion()
        {
            var dataAccess = IoCManager.Resolve<IDataAccess>();
            var routeFinishedAchievementDto = new RouteFinishedAchievementDto(123, "title", "description", "imageUrl", 124, 25, 456);

            dataAccess.InTransaction<object>(transaction =>
            {
                var routeFinishedAchievement = AchievementConverter.Convert(new List<AchievementDto>
                {
                    routeFinishedAchievementDto
                }, transaction.DataAccess)[0] as RouteFinishedAchievement;

                Assert.NotNull(routeFinishedAchievement);
                Assert.AreEqual(routeFinishedAchievementDto.Id.ToString(), routeFinishedAchievement.Id);
                Assert.AreEqual(routeFinishedAchievementDto.Description, routeFinishedAchievement.Description);
                Assert.AreEqual(routeFinishedAchievementDto.ThumbnailUrl, routeFinishedAchievement.ThumbnailUrl);
                Assert.AreEqual(routeFinishedAchievementDto.Title, routeFinishedAchievement.Title);
                Assert.AreEqual(routeFinishedAchievementDto.NextId?.ToString(), routeFinishedAchievement.NextId);
                Assert.AreEqual(routeFinishedAchievementDto.Points, routeFinishedAchievement.Points);
                Assert.AreEqual(routeFinishedAchievementDto.RouteId, routeFinishedAchievement.RouteId);

                return null;
            });
        }

        [Test, Category("UnitTest")]
        public void Test_ExhibitsVisitedAchievementConversion()
        {
            var dataAccess = IoCManager.Resolve<IDataAccess>();
            var exhibitsVisitedAchievementDto = new ExhibitsVisitedAchievementDto(234, "title", "description", "imageUrl", 235, 10, 25);

            dataAccess.InTransaction<object>(transaction =>
            {
                var exhibitsVisitedAchievement = AchievementConverter.Convert(new List<AchievementDto>
                {
                    exhibitsVisitedAchievementDto
                }, transaction.DataAccess)[0] as ExhibitsVisitedAchievement;

                Assert.NotNull(exhibitsVisitedAchievement);
                Assert.AreEqual(exhibitsVisitedAchievementDto.Id.ToString(), exhibitsVisitedAchievement.Id);
                Assert.AreEqual(exhibitsVisitedAchievementDto.Description, exhibitsVisitedAchievement.Description);
                Assert.AreEqual(exhibitsVisitedAchievementDto.ThumbnailUrl, exhibitsVisitedAchievement.ThumbnailUrl);
                Assert.AreEqual(exhibitsVisitedAchievementDto.Title, exhibitsVisitedAchievement.Title);
                Assert.AreEqual(exhibitsVisitedAchievementDto.Count, exhibitsVisitedAchievement.Count);
                Assert.AreEqual(exhibitsVisitedAchievementDto.NextId?.ToString(), exhibitsVisitedAchievement.NextId);
                Assert.AreEqual(exhibitsVisitedAchievementDto.Points, exhibitsVisitedAchievement.Points);

                return null;
            });
        }
    }
}