using System;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.BusinessRules
{
    public class PlanosVoxDocsRules
    {
        private readonly List<string> predefinedPlanNames = new List<string>
        {
            "Plano Basico",
            "Plano Intermediario",
            "Plano Avançado"
        };

        public bool IsPlanNameValid(string planName)
        {
            if (string.IsNullOrWhiteSpace(planName))
                return false;

            return !predefinedPlanNames.Contains(planName);
        }

        public bool IsPlanPriceValid(decimal price)
        {
            return price > 0;
        }

        public bool IsPlanDurationValid(int duration)
        {
            return duration > 0;
        }

        public void ValidatePlan(string name, decimal price, int duration)
        {
            if (!IsPlanNameValid(name))
                throw new ArgumentException("The plan name is invalid or already exists.");

            if (!IsPlanPriceValid(price))
                throw new ArgumentException("The plan price must be greater than zero.");

            if (!IsPlanDurationValid(duration))
                throw new ArgumentException("The plan duration must be greater than zero.");
        }
    }
}