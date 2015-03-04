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
using AutoMapper;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Controllers
{
	public partial class PlayerController : Controller
	{
		internal const int NUMBER_OF_RECENT_GAMES_TO_RETRIEVE = 10;
		internal const int NUMBER_OF_TOP_PLAYERS_TO_RETRIEVE = 25;
		internal const int NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_RETRIEVE = 25;
		internal const string EMAIL_SUBJECT_PLAYER_INVITATION = "Invitation from {0}";
		internal const string EMAIL_BODY_PLAYER_INVITATION = "Check out this gaming group I created to record the results of our board games!";

		internal IDataContext dataContext;
		internal IGameResultViewModelBuilder builder;
		internal IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
		internal IShowingXResultsMessageBuilder showingXResultsMessageBuilder;
		internal IPlayerSaver playerSaver;
		internal IPlayerRetriever playerRetriever;
		internal IPlayerInviter playerInviter;
		internal IPlayerEditViewModelBuilder playerEditViewModelBuilder;
		internal IPlayerSummaryBuilder playerSummaryBuilder;
		internal ITopPlayerViewModelBuilder topPlayerViewModelBuilder;
		internal INemesisHistoryRetriever nemesisHistoryRetriever;
		internal INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder;

		public PlayerController(IDataContext dataContext,
			IGameResultViewModelBuilder builder,
			IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
			IShowingXResultsMessageBuilder showingXResultsMessageBuilder,
			IPlayerSaver playerSaver,
			IPlayerRetriever playerRetriever,
			IPlayerInviter playerInviter,
			IPlayerEditViewModelBuilder playerEditViewModelBuilder,
			IPlayerSummaryBuilder playerSummaryBuilder,
			ITopPlayerViewModelBuilder topPlayerViewModelBuilder,
			INemesisHistoryRetriever nemesisHistoryRetriever,
			INemesisChangeViewModelBuilder nemesisChangeViewModelBuilder)
		{
			this.dataContext = dataContext;
			this.builder = builder;
			this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
			this.showingXResultsMessageBuilder = showingXResultsMessageBuilder;
			this.playerSaver = playerSaver;
			this.playerRetriever = playerRetriever;
			this.playerInviter = playerInviter;
			this.playerEditViewModelBuilder = playerEditViewModelBuilder;
			this.playerSummaryBuilder = playerSummaryBuilder;
			this.topPlayerViewModelBuilder = topPlayerViewModelBuilder;
			this.nemesisHistoryRetriever = nemesisHistoryRetriever;
			this.nemesisChangeViewModelBuilder = nemesisChangeViewModelBuilder;
		}

		// GET: /Player/Details/5
		[UserContextAttribute(RequiresGamingGroup = false)]
		public virtual ActionResult Details(int? id, ApplicationUser currentUser)
		{
			if (!id.HasValue)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			PlayerDetails player = playerRetriever.GetPlayerDetails(id.Value, NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);

			if (player == null)
			{
				return new HttpNotFoundResult();
			}

			var fullUrl = this.Url.Action(MVC.Player.ActionNames.Details, MVC.Player.Name, new { id }, this.Request.Url.Scheme) + "#minions";
			PlayerDetailsViewModel playerDetailsViewModel = playerDetailsViewModelBuilder.Build(player, fullUrl, currentUser);

			ViewBag.RecentGamesMessage = showingXResultsMessageBuilder.BuildMessage(
				NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
				player.PlayerGameResults.Count);

			return View(MVC.Player.Views.Details, playerDetailsViewModel);
		}

		// GET: /Player/SavePlayer
		[System.Web.Mvc.Authorize]
		public virtual ActionResult SavePlayer()
		{
			return View(MVC.Player.Views._CreateOrUpdatePartial, new Player());
		}

		// GET: /Player/Create
		[System.Web.Mvc.Authorize]
		public virtual ActionResult Create()
		{
			return View(MVC.Player.Views.Create, new Player());
		}

		// GET: /Player/InvitePlayer/5
		[System.Web.Mvc.Authorize]
		[UserContextAttribute]
		public virtual ActionResult InvitePlayer(int id, ApplicationUser currentUser)
		{
			PlayerDetails playerDetails;

			try
			{
				playerDetails = playerRetriever.GetPlayerDetails(id, 0);
			}
			catch (KeyNotFoundException)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}

			string emailSubject = string.Format(EMAIL_SUBJECT_PLAYER_INVITATION, currentUser.UserName);

			var playerInvitationViewModel = new PlayerInvitationViewModel
			{
				PlayerId = playerDetails.Id,
				PlayerName = playerDetails.Name,
				EmailSubject = emailSubject,
				EmailBody = EMAIL_BODY_PLAYER_INVITATION
			};
			return View(MVC.Player.Views.InvitePlayer, playerInvitationViewModel);
		}

		[HttpGet]
		public virtual ActionResult ShowTopPlayers()
		{
			var topPlayers = playerSummaryBuilder.GetTopPlayers(NUMBER_OF_TOP_PLAYERS_TO_RETRIEVE);
			var topPlayersViewModels = topPlayers.Select(topPlayer => this.topPlayerViewModelBuilder.Build(topPlayer)).ToList();
			return View(MVC.Player.Views.TopPlayers, topPlayersViewModels);
		}

		[HttpGet]
		public virtual ActionResult ShowRecentNemesisChanges()
		{
			var recentNemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(NUMBER_OF_RECENT_NEMESIS_CHANGES_TO_RETRIEVE);
			var recentNemesisChangesViewModels = this.nemesisChangeViewModelBuilder.Build(recentNemesisChanges).ToList();
			return View(MVC.Player.Views.RecentNemesisChanges, recentNemesisChangesViewModels);
		}

		[System.Web.Mvc.HttpPost]
		[System.Web.Mvc.Authorize]
		[UserContextAttribute]
		public virtual ActionResult InvitePlayer(PlayerInvitationViewModel playerInvitationViewModel, ApplicationUser currentUser)
		{
			PlayerInvitation playerInvitation = new PlayerInvitation
			{
				InvitedPlayerId = playerInvitationViewModel.PlayerId,
				InvitedPlayerEmail = playerInvitationViewModel.EmailAddress.Trim(),
				EmailSubject = playerInvitationViewModel.EmailSubject,
				CustomEmailMessage = playerInvitationViewModel.EmailBody
			};

			playerInviter.InvitePlayer(playerInvitation, currentUser);

			return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name));
		}

		[System.Web.Mvc.Authorize]
		[System.Web.Mvc.HttpPost]
		[UserContextAttribute]
		public virtual ActionResult Save(Player model, ApplicationUser currentUser)
		{
			if (!Request.IsAjaxRequest())
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (ModelState.IsValid)
			{
				try
				{
					model.Name = model.Name.Trim();
					Player player = playerSaver.Save(model, currentUser);
					return Json(player, JsonRequestBehavior.AllowGet);
				}
				catch (PlayerAlreadyExistsException playerAlreadyExistsException)
				{
					return new HttpStatusCodeResult(HttpStatusCode.Conflict, playerAlreadyExistsException.Message);
				}
			}

			return new HttpStatusCodeResult(HttpStatusCode.NotModified);
		}

		// GET: /Player/Edit/5
		[System.Web.Mvc.Authorize]
		public virtual ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			PlayerDetails player;
			try
			{
				player = playerRetriever.GetPlayerDetails(id.Value, 0);
			}
			catch (UnauthorizedAccessException)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
			}
			catch (KeyNotFoundException)
			{
				return new HttpStatusCodeResult(HttpStatusCode.NotFound);
			}
			var playerEditViewModel = new PlayerEditViewModel
			{
				Active = player.Active,
				Id = player.Id,
				GamingGroupId = player.GamingGroupId,
				Name = player.Name
			};
			return View(MVC.Player.Views.Edit, playerEditViewModel);
		}

		// POST: /Player/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[System.Web.Mvc.Authorize]
		[System.Web.Mvc.HttpPost]
		[ValidateAntiForgeryToken]
		[UserContextAttribute]
		public virtual ActionResult Edit([Bind(Include = "Id,Name,Active,GamingGroupId")] Player player, ApplicationUser currentUser)
		{
			if (ModelState.IsValid)
			{
				player.Name = player.Name.Trim();
				playerSaver.Save(player, currentUser);
				return new RedirectResult(Url.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name)
										  + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS);
			}

			var playerEditViewModel = playerEditViewModelBuilder.Build(player);

			return View(MVC.Player.Views.Edit, playerEditViewModel);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
