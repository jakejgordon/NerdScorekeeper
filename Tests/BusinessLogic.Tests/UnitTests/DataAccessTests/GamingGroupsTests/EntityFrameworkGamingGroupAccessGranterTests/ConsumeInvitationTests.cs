﻿using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class ConsumeInvitationTests : EntityFrameworkGamingGroupAccessGranterTestBase
    {
        [Test]
        public void ItSetsTheDateRegistered()
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation();

            gamingGroupAccessGranter.ConsumeInvitation(invitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GamingGroupInvitation>.Matches(invite => invite.DateRegistered.Value.Date == DateTime.UtcNow.Date),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsTheRegisteredUserId()
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation();

            gamingGroupAccessGranter.ConsumeInvitation(invitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GamingGroupInvitation>.Matches(invite => invite.RegisteredUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
