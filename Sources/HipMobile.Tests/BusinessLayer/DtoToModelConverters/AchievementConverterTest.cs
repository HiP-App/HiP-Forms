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
            sub.CreateObject<ExhibitsVisitedAchievement>("").ReturnsForAnyArgs(Substitute.For<ExhibitsVisitedAchievement>());
            sub.CreateObject<RouteFinishedAchievement>("").ReturnsForAnyArgs(Substitute.For<RouteFinishedAchievement>());
            IoCManager.RegisterInstance(typeof(IDataAccess), sub);
        }
        
        [Test, Category("UnitTest")]
        public void Test_RouteFinishedAchievementConversion()
        {
            var routeFinishedAchievementDto = new RouteFinishedAchievementDto(123, "title", "description", "imageUrl", 124);
            var routeFinishedAchievement = AchievementConverter.Convert(new List<AchievementDto>
            {
                routeFinishedAchievementDto
            })[0] as RouteFinishedAchievement;
            Assert.NotNull(routeFinishedAchievement);
            Assert.AreEqual(routeFinishedAchievementDto.Id.ToString(), routeFinishedAchievement.Id);
            Assert.AreEqual(routeFinishedAchievementDto.Description, routeFinishedAchievement.Description);
            Assert.AreEqual(routeFinishedAchievementDto.ImageUrl, routeFinishedAchievement.ImageUrl);
            Assert.AreEqual(routeFinishedAchievementDto.Title, routeFinishedAchievement.Title);
            Assert.AreEqual(routeFinishedAchievementDto.NextId.ToString(), routeFinishedAchievement.NextId);
        }

        [Test, Category("UnitTest")]
        public void Test_ExhibitsVisitedAchievementConversion()
        {
            var exhibitsVisitedAchievementDto = new ExhibitsVisitedAchievementDto(234, "title", "description", "imageUrl", 235, 10);
            var exhibitsVisitedAchievement = AchievementConverter.Convert(new List<AchievementDto>
            {
                exhibitsVisitedAchievementDto
            })[0] as ExhibitsVisitedAchievement;
            Assert.NotNull(exhibitsVisitedAchievement);
            Assert.AreEqual(exhibitsVisitedAchievementDto.Id.ToString(), exhibitsVisitedAchievement.Id);
            Assert.AreEqual(exhibitsVisitedAchievementDto.Description, exhibitsVisitedAchievement.Description);
            Assert.AreEqual(exhibitsVisitedAchievementDto.ImageUrl, exhibitsVisitedAchievement.ImageUrl);
            Assert.AreEqual(exhibitsVisitedAchievementDto.Title, exhibitsVisitedAchievement.Title);
            Assert.AreEqual(exhibitsVisitedAchievementDto.Count, exhibitsVisitedAchievement.Count);
            Assert.AreEqual(exhibitsVisitedAchievementDto.NextId.ToString(), exhibitsVisitedAchievement.NextId);
        }
    }
}