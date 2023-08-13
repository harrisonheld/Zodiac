using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using static UnityEditor.Progress;
using static UnityEngine.ParticleSystem;

public class Dice
{
    private static Random random = new Random();

    /// <summary>
    /// Le functions:
    /// <list type="bullet">
    /// <item>
    ///     <term>n</term>
    ///     <description>A constant</description>
    /// </item>
    /// <item>
    ///     <term>XdY</term>
    ///     <description>Roll a Y-sided die X times</description>
    /// </item>
    /// <item>
    ///     <term>+</term>
    ///     <description>Addition</description>
    /// </item>
    /// <item>
    ///     <term>-</term>
    ///     <description>Subtraction</description>
    /// </item>
    /// </list>
    /// Example: 2d6 + 1d4 - 1
    /// </summary>
    public static int Parse(string expression)
    {
        // Remove whitespace
        expression = Regex.Replace(expression, @"\s+", "");

        // Parse dice expressions and operators
        string[] terms = Regex.Split(expression, @"([-+])");
        int result = 0;
        char currentOperator = '+';

        foreach (string term in terms)
        {
            if (term == "+" || term == "-")
            {
                currentOperator = term[0];
            }
            else
            {
                int value = EvaluateTerm(term);
                if (currentOperator == '+')
                    result += value;
                else if (currentOperator == '-')
                    result -= value;
            }
        }

        return result;
    }

    static int EvaluateTerm(string term)
    {
        if (Regex.IsMatch(term, @"\d+d\d+"))
        {
            return RollDice(term);
        }
        else
        {
            return int.Parse(term);
        }
    }

    static int RollDice(string diceExpression)
    {
        Match match = Regex.Match(diceExpression, @"(\d+)d(\d+)");
        int numDice = int.Parse(match.Groups[1].Value);
        int numSides = int.Parse(match.Groups[2].Value);
        int result = 0;

        for (int i = 0; i < numDice; i++)
        {
            result += random.Next(1, numSides + 1);
        }

        return result;
    }
}