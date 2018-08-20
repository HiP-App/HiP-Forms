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

using System;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.UI.AudioPlayer;
using PaderbornUniversity.SILab.Hip.Mobile.UI.Navigation;
using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileUITests.ViewModels.Views
{
    class AudioToolbarViewModelTest
    {
        [TestCase(true), Category("UnitTest")]
        [TestCase(false), Category("UnitTest")]
        public void Creation_PropertiesFilled(bool automaticAudioStart)
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            Assert.IsTrue(Math.Abs(sut.MaxAudioProgress - 42) < 0.01);
            Assert.IsFalse(sut.IsAudioPlaying);
            Assert.NotNull(sut.CaptionCommand);
            Assert.NotNull(sut.PauseCommand);
            Assert.NotNull(sut.PlayCommand);
            if (automaticAudioStart)
            {
                audioPlayer.ReceivedWithAnyArgs().Play();
            }
            else
            {
                audioPlayer.DidNotReceiveWithAnyArgs().Play();
            }
        }

        [Test, Category("UnitTest")]
        public void Command_Play()
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            sut.PlayCommand.Execute(null);

            audioPlayer.ReceivedWithAnyArgs().Play();
        }

        [Test, Category("UnitTest")]
        public void Command_Pause()
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            sut.PauseCommand.Execute(null);

            audioPlayer.ReceivedWithAnyArgs().Pause();
        }

        [Test, Category("UnitTest")]
        public void Command_Subtitles()
        {
            var sut = CreateSystemUnderTest();
            var navigationService = IoCManager.Resolve<INavigationService>();

            sut.CaptionCommand.Execute(null);

            navigationService.ReceivedWithAnyArgs().PushAsync(null);
        }

        [Test, Category("UnitTest")]
        public void AudioPlayer_IsPlayingChanged()
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            Assert.IsFalse(sut.IsAudioPlaying);
            audioPlayer.IsPlayingChanged += Raise.Event<IsPlayingDelegate>(true);
            Assert.IsTrue(sut.IsAudioPlaying);
            audioPlayer.IsPlayingChanged += Raise.Event<IsPlayingDelegate>(false);
            Assert.IsFalse(sut.IsAudioPlaying);
        }

        [Test, Category("UnitTest")]
        public void AudioPlayer_IsCompleted()
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            audioPlayer.IsPlayingChanged += Raise.Event<IsPlayingDelegate>(true);
            Assert.IsTrue(sut.IsAudioPlaying);
            audioPlayer.AudioCompleted += Raise.Event<AudioCompletedDelegate>();
            Assert.IsFalse(sut.IsAudioPlaying);
        }

        [Test, Category("UnitTest")]
        public void OnDisappearing_Stopped()
        {
            var sut = CreateSystemUnderTest();
            var audioPlayer = IoCManager.Resolve<IAudioPlayer>();

            sut.OnDisappearing();
            audioPlayer.ReceivedWithAnyArgs().Stop();
        }

        #region Helper Methods

        private AudioToolbarViewModel CreateSystemUnderTest()
        {
            IoCManager.Clear();
            var audioPlayer = Substitute.For<IAudioPlayer>();
            IoCManager.RegisterInstance(typeof(INavigationService), Substitute.For<INavigationService>());
            IoCManager.RegisterInstance(typeof(IAudioPlayer), audioPlayer);
            IoCManager.RegisterInstance(typeof(IMediaFileManager), new DummyMediaFileManager());
            audioPlayer.MaximumProgress.Returns(42);

            var audio = Substitute.For<Audio>();
            var viewmodel = new AudioToolbarViewModel("Title", true);
            viewmodel.SetNewAudioFile(audio);

            return viewmodel;
        }

        #endregion
    }
}