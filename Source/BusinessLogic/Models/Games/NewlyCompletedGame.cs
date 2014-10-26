﻿using BusinessLogic.Models.Games.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BusinessLogic.Models.Games
{
    public class NewlyCompletedGame
    {
        [Required]
        public int? GameDefinitionId { get; set; }

        //TODO my validation attribute works for a List<PlayerRank>, not PlayerGameResult. How to get validation that works for
        // both a NewlyCompletedGame and a PlayedGame....
        [PlayerRankValidationAttribute]
        [Required]
        public List<PlayerRank> PlayerRanks { get; set; }
    }
}
