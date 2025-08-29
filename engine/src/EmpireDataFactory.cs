
using System.Text.Json;

// The key String is the planet
// The value SortedSet<int> is the list of days on which bounty hunters are on the planet.
using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedSet<int>>;

public struct EmpireData : IEquatable<EmpireData>
{
    public BountyHuntersMap m_bountyHuntersPresence { get; set; }
    public int m_countdown { get; set; }

    public bool Equals(EmpireData other)
    {
        if (m_countdown != other.m_countdown)
            return false;

        // Null checks
        if (m_bountyHuntersPresence == null && other.m_bountyHuntersPresence == null)
            return true;
        if (m_bountyHuntersPresence == null || other.m_bountyHuntersPresence == null)
            return false;

        // key value check
        if (m_bountyHuntersPresence.Count != other.m_bountyHuntersPresence.Count)
            return false;
        foreach (var pair in m_bountyHuntersPresence)
            if (!pair.Value.SetEquals(other.m_bountyHuntersPresence[pair.Key]))
                return false;
        return true;


    }
    public override bool Equals(object? obj)
    {
        return obj is EmpireData other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(m_countdown, m_bountyHuntersPresence?.Count ?? 0);
    }
}
public class EmpireDataFactory
{
    public static EmpireData Build(ref EmpireDataDto empireDto)
    {
        var bountyHuntersMap = new BountyHuntersMap();

        if (empireDto.m_bountyHunters != null)
        {
            // Group bounty hunters by planet
            foreach (var hunterPresence in empireDto.m_bountyHunters)
            {
                if (string.IsNullOrEmpty(hunterPresence.m_planet))
                    continue;

                if (!bountyHuntersMap.ContainsKey(hunterPresence.m_planet))
                {
                    bountyHuntersMap[hunterPresence.m_planet] = new SortedSet<int>();
                }

                bountyHuntersMap[hunterPresence.m_planet].Add(hunterPresence.m_day);
            }
        }

        return new EmpireData
        {
            m_bountyHuntersPresence = bountyHuntersMap,
            m_countdown = empireDto.m_countdown
        };
    }
}
