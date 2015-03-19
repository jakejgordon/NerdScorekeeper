﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.Points
{
    public class PointsCalculator
    {
        public static readonly Dictionary<int, int> FIBONACCI_N_PLUS_2 = new Dictionary<int, int>
        {
            {1, 1},
            {2, 2},
            {3, 3},
            {4, 5},
            {5, 8},
            {6, 13},
            {7, 21},
            {8, 34},
            {9, 55},
            {10, 89},
            {11, 144},
            {12, 233},
            {13, 377},
            {14, 610},
            {15, 987}
        };

        internal const int DEFAULT_POINTS_PER_PLAYER_WHEN_EVERYONE_LOSES = 2;
        internal const int POINTS_PER_PLAYER = 10;

        internal static Dictionary<int, int> CalculatePoints(IList<PlayerRank> playerRanks)
        {
            Dictionary<int, int> playerToPoints = new Dictionary<int, int>(playerRanks.Count);

            if (EveryoneLost(playerRanks))
            {
                GiveEveryoneTwoPoints(playerRanks, playerToPoints);
            }
            else
            {
                CalculatePointsForPlayers(playerRanks, playerToPoints);
            }

            return playerToPoints;
        }

        private static void CalculatePointsForPlayers(IList<PlayerRank> playerRanks, Dictionary<int, int> playerToPoints)
        {
            int totalNumberOfPlayers = playerRanks.Count;
            int totalPointsToAllocate = totalNumberOfPlayers * POINTS_PER_PLAYER;
            decimal denominator = FibonacciSum(totalNumberOfPlayers);

            const int FIBONACCI_OFFSET = 1;
            int highestRankSlotNotConsumed = 1;
            
            for (int rank = 1; rank <= totalNumberOfPlayers; rank++)
            {
                List<PlayerRank> playersWithThisRank = playerRanks.Where(x => x.GameRank == rank).ToList();
                int numberOfPlayersWithThisRank = playersWithThisRank.Count;
                if (numberOfPlayersWithThisRank > 0)
                {
                    int finoacciEndIndex = totalNumberOfPlayers + FIBONACCI_OFFSET - highestRankSlotNotConsumed;
                    int fibonacciStartIndex = finoacciEndIndex + FIBONACCI_OFFSET - numberOfPlayersWithThisRank;
                    decimal numerator = FibonacciSum(fibonacciStartIndex, finoacciEndIndex) / numberOfPlayersWithThisRank;
                    int points = RoundUpIfNonNegligible(totalPointsToAllocate * (numerator / denominator));

                    foreach (var playerRank in playersWithThisRank)
                    {
                        playerToPoints.Add(playerRank.PlayerId, points);
                    }
                }

                highestRankSlotNotConsumed += numberOfPlayersWithThisRank;
            }
        }

        private static void GiveEveryoneTwoPoints(IList<PlayerRank> playerRanks, Dictionary<int, int> playerToPoints)
        {
            foreach (var playerRank in playerRanks)
            {
                playerToPoints.Add(playerRank.PlayerId, DEFAULT_POINTS_PER_PLAYER_WHEN_EVERYONE_LOSES);
            }
        }

        private static bool EveryoneLost(IList<PlayerRank> playerRanks)
        {
            return playerRanks.All(x => x.GameRank != 1);
        }

        private static decimal FibonacciSum(int fibonacciStartIndex, int finoacciEndIndex)
        {
            return FIBONACCI_N_PLUS_2.Where(fibonacci => fibonacci.Key >= fibonacciStartIndex 
                && fibonacci.Key <= finoacciEndIndex).Sum(x => x.Value);
        }

        private static decimal FibonacciSum(int endIndex)
        {
            return FibonacciSum(1, endIndex);
        }

        private static int RoundUpIfNonNegligible(decimal valueToRound)
        {
            decimal floor = Math.Floor(valueToRound);
            return (int)((valueToRound - floor) > (decimal)0.1 ? floor + 1 : floor);
        }

        //10 * (numberOfPlayers) * (fibonacciOf(numberOfPlayers + 1 - r) / fibonacciSumFrom(2, numberOfPlayers))
        //still need to make sure that ranked bucket groupings work correctly.
    }
}
