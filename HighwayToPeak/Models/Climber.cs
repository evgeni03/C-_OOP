using HighwayToPeak.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Models
{
    public abstract class Climber : IClimber
    {
        private string name;
        private int stamina;
        private readonly List<string> conqueredPeaks;

        public Climber(string name, int stamina)
        {
            this.Name = name;
            this.Stamina = stamina;
            this.conqueredPeaks = new List<string>();
        }

        public string Name 
        { 
            get => name;
           private set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("Climber's name cannot be null or whitespace.");
                }
                name = value;
            }
        }

        public int Stamina 
        { get => stamina;
          protected set
            {
                if(value > 10)
                {
                    value = 10;
                }
                else if(value < 0)
                {
                    value = 0;
                }
                stamina = value;
            }
        }

        public IReadOnlyCollection<string> ConqueredPeaks => conqueredPeaks.AsReadOnly();

        public void Climb(IPeak peak)
        {
            int staminaCost = CalculateStaminaCost(peak.DifficultyLevel);
            Stamina -= staminaCost;

            if (!conqueredPeaks.Contains(peak.Name))
            {
                conqueredPeaks.Add(peak.Name);
            }
        }

        public abstract void Rest(int daysCount);

        private int CalculateStaminaCost(string difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case "Extreme":
                    return 6;
                case "Hard":
                    return 4;
                case "Moderate":
                    return 2;
                default:
                    return 0;
            }
        }
        public override string ToString()
        {
           StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{GetType().Name} - Name: {Name}, Stamina: {Stamina}");
           
            return sb.ToString().TrimEnd();

        }
    }
    
}
