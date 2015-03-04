﻿#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class InvitePlayerHttpGetTests : PlayerControllerTestBase
    {
        private PlayerDetails playerDetails;
        private int playerId = 915;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            playerDetails = new PlayerDetails
            {
                Name = "player name",
                Id = playerId
            };
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                              .Return(playerDetails);
        }

        [Test]
        public void ItReturnsNotFoundHttpStatusWhenInvalidPlayerIdGiven()
        {
            int invalidPlayerId = -1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(invalidPlayerId, 0))
                               .Throw(new KeyNotFoundException());

            HttpStatusCodeResult actualResult = playerController.InvitePlayer(-1, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }

        [Test]
        public void ItSetsThePlayerNameAndIdOnTheViewModel()
        {
            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(playerDetails.Name, ((PlayerInvitationViewModel)viewResult.Model).PlayerName);
            Assert.AreEqual(playerDetails.Id, ((PlayerInvitationViewModel)viewResult.Model).PlayerId);
        }

        [Test]
        public void ItDefaultsTheEmailSubjectToSayThatSomeoneHasInvitedThem()
        {
            string expectedEmailSubject = string.Format(PlayerController.EMAIL_SUBJECT_PLAYER_INVITATION, currentUser.UserName);

            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(expectedEmailSubject, ((PlayerInvitationViewModel)viewResult.Model).EmailSubject);
        }

        [Test]
        public void ItDefaultsTheEmailBodyToSayThatSomeoneHasInvitedThemToNemeStats()
        {
            string expectedEmailBody = string.Format(
                PlayerController.EMAIL_BODY_PLAYER_INVITATION,
                playerDetails.GamingGroupName);

            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(expectedEmailBody, ((PlayerInvitationViewModel)viewResult.Model).EmailBody);
        }
    }
}
