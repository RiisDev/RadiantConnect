using RadiantConnect.Network.PreGameEndpoints.DataTypes;
// ReSharper disable All

namespace RadiantConnect.Network.PreGameEndpoints;

public class PreGameEndpoints(Initiator initiator)
{
    internal static readonly Dictionary<Agent, string> AgentToAgentId = new()
    {
        { Agent.Astra, "41fb69c1-4189-7b37-f117-bcaf1e96f1bf" },
        { Agent.Breach, "5f8d3a7f-467b-97f3-062c-13acf203c006" },
        { Agent.Brimstone, "9f0d8ba9-4140-b941-57d3-a7ad57c6b417" },
        { Agent.Chamber, "22697a3d-45bf-8dd7-4fec-84a9e28c69d7" },
        { Agent.Clove, "1dbf2edd-4729-0984-3115-daa5eed44993" },
        { Agent.Cypher, "117ed9e3-49f3-6512-3ccf-0cada7e3823b" },
        { Agent.Deadlock, "cc8b64c8-4b25-4ff9-6e7f-37b4da43d235" },
        { Agent.Fade, "dade69b4-4f5a-8528-247b-219e5a1facd6" },
        { Agent.Gekko, "e370fa57-4757-3604-3648-499e1f642d3f" },
        { Agent.Harbor, "95b78ed7-4637-86d9-7e41-71ba8c293152" },
        { Agent.ISO, "0e38b510-41a8-5780-5e8f-568b2a4f2d6c" },
        { Agent.Jett, "add6443a-41bd-e414-f6ad-e58d267f4e95" },
        { Agent.KAYO, "601dbbe7-43ce-be57-2a40-4abd24953621" },
        { Agent.Killjoy, "1e58de9c-4950-5125-93e9-a0aee9f98746" },
        { Agent.Neon, "bb2a4828-46eb-8cd1-e765-15848195d751" },
        { Agent.Omen, "8e253930-4c05-31dd-1b6c-968525494517" },
        { Agent.Phoenix, "eb93336a-449b-9c1b-0a54-a891f7921d69" },
        { Agent.Raze, "f94c3b30-42be-e959-889c-5aa313dba261" },
        { Agent.Reyna, "a3bfb853-43b2-7238-a4f1-ad90e9e46bcc" },
        { Agent.Sage, "569fdd95-4d10-43ab-ca70-79becc718b46" },
        { Agent.Skye, "6f2a04ca-43e0-be17-7f36-b3908627744d" },
        { Agent.Sova, "320b2a48-4d9b-a075-30f1-1f93a9b638fa" },
        { Agent.Tejo, "b444168c-4e35-8076-db47-ef9bf368f384" },
        { Agent.Viper, "707eab51-4836-f488-046a-cda6bf494859" },
        { Agent.Vyse, "efba5359-4016-a1e5-7626-b1ae76895940" },
        { Agent.Yoru, "7f94d92c-4234-0a36-9646-3a87eb8b5c89" }
    };

    public enum Agent
    {
        Astra,
        Breach,
        Brimstone,
        Chamber,
        Clove,
        Cypher,
        Deadlock,
        Fade,
        Gekko,
        Harbor,
        ISO,
        Jett,
        KAYO,
        Killjoy,
        Neon,
        Omen,
        Phoenix,
        Raze,
        Reyna,
        Sage,
        Skye,
        Sova,
        Tejo,
        Viper,
        Vyse,
        Yoru,
    }

    internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

    public async Task<PreGamePlayer?> FetchPreGamePlayerAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PreGamePlayer>(Url, $"/pregame/v1/players/{userId}");
    }

    public async Task<PreGameMatch?> FetchPreGameMatchAsync(string matchId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}");
    }

    public async Task<GameLoadout?> FetchPreGameLoadoutAsync(string matchId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/pregame/v1/matches/{matchId}/loadouts");
    }

    public async Task<PreGameMatch?> SelectCharacterAsync(string matchId, Agent agent)
    {
        return await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}/select/{AgentToAgentId[agent]}");
    }

    public async Task<PreGameMatch?> LockCharacterAsync(string matchId, Agent agent)
    {
        return await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}/lock/{AgentToAgentId[agent]}");
    }

    public async Task QuitGameAsync(string matchId)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Post, Url, $"/pregame/v1/matches/{matchId}/quit");
    }
}