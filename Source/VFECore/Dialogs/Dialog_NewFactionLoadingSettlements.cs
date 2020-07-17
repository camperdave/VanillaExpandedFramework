﻿using System;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VFECore
{
    public class Dialog_NewFactionLoadingSettlements : Window
    {
        private readonly Action<int, int> spawnCallback;

        private const float newFactionSettlementFactor = 0.7f; // recommendation
        private const float settlementsPer100KTiles = 80; // average

        private int settlementsToSpawn;
        private int settlementsRecommended;
        private int distanceToSpawn;
        private int distanceRecommended;

        public static void OpenDialog(Action<int, int> spawnCallback)
        {
            Find.WindowStack.Add(new Dialog_NewFactionLoadingSettlements(spawnCallback));
        }

        private Dialog_NewFactionLoadingSettlements(Action<int, int> spawnCallback)
        {
            doCloseButton = false;
            forcePause = true;
            absorbInputAroundWindow = true;
            this.spawnCallback = spawnCallback;

            settlementsToSpawn = settlementsRecommended = GetSettlementsRecommendation();
            distanceToSpawn = distanceRecommended = SettlementProximityGoodwillUtility.MaxDist;
        }

        private static int GetSettlementsRecommendation()
        {
            int existingFactions = Find.FactionManager.AllFactionsVisible.Count();
            return GenMath.RoundRandom(Find.WorldGrid.TilesCount / 100000f * settlementsPer100KTiles / existingFactions * newFactionSettlementFactor);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect.AtZero());

            // Description
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            listing_Standard.Label($"Settlements to spawn (recommended are {settlementsRecommended}): {settlementsToSpawn}");
            settlementsToSpawn = Mathf.CeilToInt(listing_Standard.Slider(settlementsToSpawn, 0, settlementsRecommended * 2));

            listing_Standard.Label($"The minimum distance from player bases (recommended are {distanceRecommended}: {distanceToSpawn}");
            distanceToSpawn = Mathf.CeilToInt(listing_Standard.Slider(distanceToSpawn, 1, distanceToSpawn * 2f));

            if (listing_Standard.ButtonText("Spawn")) Spawn();
            if (listing_Standard.ButtonText("Cancel")) Close();
        }

        private void Spawn()
        {
            Close();
            spawnCallback(settlementsToSpawn, distanceToSpawn);
        }
    }
}
