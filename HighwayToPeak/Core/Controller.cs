using HighwayToPeak.Core.Contracts;
using HighwayToPeak.Models;
using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Core
{
    public class Controller : IController
    {
        private IRepository<IPeak> peaks;
        private IRepository<IClimber> climbers;
        private BaseCamp baseCamp;

        public Controller()
        {
            peaks = new PeakRepository();
            climbers = new ClimberRepository();
            baseCamp = new BaseCamp();
        }
        public string AddPeak(string name, int elevation, string difficultyLevel)
        {
            
            if(peaks.Get(name) != null)
            {
                return $"{name} is already added as a valid mountain destination.";
            }
            if(!IsValidDifficultyLevel(difficultyLevel) && climbers.GetType().Name == "NaturalClimber")
            {
                return $"{difficultyLevel} peaks are not allowed for international climbers.";
            }
            IPeak peak = new Peak(name, elevation, difficultyLevel);
            peaks.Add(peak);
            

            return $"{name} is allowed for international climbing. See details in PeakRepository.";
        }

        public string AttackPeak(string climberName, string peakName)
        {
            IClimber climber = climbers.Get(climberName);
            if (climber == null)
            {
                return $"Climber - {climberName}, has not arrived at the BaseCamp yet.";
            }

            IPeak peak = peaks.Get(peakName);
            if (peak == null)
            {
                return $"{peakName} is not allowed for international climbing.";
            }

            if (!baseCamp.Residents.Contains(climberName))
            {
                return $"{climberName} not found for gearing and instructions. The attack of {peakName} will be postponed.";
            }

            if (peak.DifficultyLevel.Equals("Extreme", StringComparison.OrdinalIgnoreCase) &&
                climber is NaturalClimber)
            {
                return $"{climberName} does not cover the requirements for climbing {peakName}.";
            }

            baseCamp.LeaveCamp(climberName);

            try
            {
                climber.Climb(peak);

                if (climber.Stamina == 0)
                {
                    return $"{climberName} did not return to BaseCamp.";
                }

                baseCamp.ArriveAtCamp(climberName);
                return $"{climberName} successfully conquered {peakName} and returned to BaseCamp.";
            }
            catch
            {
                return $"{climberName} did not return to BaseCamp.";
            }
        }

        public string BaseCampReport()
        {
            if (baseCamp.Residents.Count == 0)
            {
                return "BaseCamp is currently empty.";
            }

            StringBuilder report = new StringBuilder("BaseCamp residents:");
            report.AppendLine();

            foreach (var climberName in baseCamp.Residents)
            {
                IClimber climber = climbers.Get(climberName);
                if (climber != null)
                {
                   
                    report.AppendLine($"Name: {climber.Name}, Stamina: {climber.Stamina}, Count of Conquered Peaks: {climber.ConqueredPeaks.Count}");
                }
            }

            return report.ToString();
        }

        public string CampRecovery(string climberName, int daysToRecover)
        {
            IClimber climber = climbers.Get(climberName);

            if (climber == null || !baseCamp.Residents.Contains(climberName))
            {
                return $"{climberName} not found at the BaseCamp.";
            }

            if (climber.Stamina == 10)
            {
                return $"{climberName} has no need of recovery.";
            }

            climber.Rest(daysToRecover);

            return $"{climberName} has been recovering for {daysToRecover} days and is ready to attack the mountain.";
        }

        public string NewClimberAtCamp(string name, bool isOxygenUsed)
        {
           
            if (climbers.Get(name) != null)
            {
                return $"{name} is a participant in {nameof(ClimberRepository)} and cannot be duplicated.";
            }
            IClimber climber = isOxygenUsed
        ? (IClimber)new OxygenClimber(name)
        : new NaturalClimber(name);

            climbers.Add(climber);
            baseCamp.ArriveAtCamp(name);

            return $"{name} has arrived at the BaseCamp and will wait for the best conditions.";
        }

        public string OverallStatistics()
        {
            var climbersOrdered = climbers.All.OrderByDescending(c => c.ConqueredPeaks.Count)
                               .ThenBy(c => c.Name)
                               .ToList();

            StringBuilder statistics = new StringBuilder("***Highway-To-Peak***");
            statistics.AppendLine();

            foreach (var climber in climbersOrdered)
            {
                statistics.AppendLine($"{climber}");

                var conqueredPeaksOrdered = climber.ConqueredPeaks
                                                   .Select(peakName => peaks.Get(peakName))
                                                   .OrderByDescending(peak => peak?.Elevation)
                                                   .ToList();

                var conqueredPeaksCount = conqueredPeaksOrdered.Count;

                if (conqueredPeaksCount > 0)
                {
                    statistics.AppendLine($"Peaks conquered: {conqueredPeaksCount}");
                    foreach (var peak in conqueredPeaksOrdered)
                    {
                        if (peak != null)
                        {
                            statistics.AppendLine($"Peak: {peak.Name} -> Elevation: {peak.Elevation}, Difficulty: {peak.DifficultyLevel}");
                        }
                    }
                }
                else
                {
                    statistics.AppendLine("No peaks conquered.");
                }
            }

            return statistics.ToString().TrimEnd();
        }
        private bool IsValidDifficultyLevel(string difficultyLevel)
        {
            return difficultyLevel.Equals("Extreme", StringComparison.OrdinalIgnoreCase) ||
                   difficultyLevel.Equals("Hard", StringComparison.OrdinalIgnoreCase) ||
                   difficultyLevel.Equals("Moderate", StringComparison.OrdinalIgnoreCase);
        }
    }
}
