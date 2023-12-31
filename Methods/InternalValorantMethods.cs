using System.Diagnostics;
namespace RadiantConnect.Methods;
// ReSharper disable All

public class InternalValorantMethods
{
    public static bool IsValorantProcessRunning() { return Process.GetProcessesByName("VALORANT").Length > 0; }

    internal static readonly Dictionary<string, string> GamePodsDictionary = new()
    {
        {"aresqa.aws-rclusterprod-use1-1.dev1-gp-ashburn-1", "Ashburn"},
        {"aresriot.aws-mes1-prod.eu-gp-bahrain-1", "Bahrain"}, {"aresriot.aws-mes1-prod.ext1-gp-bahrain-1", "Bahrain"}, {"aresriot.aws-rclusterprod-mes1-1.eu-gp-bahrain-awsedge-1", "Bahrain"}, {"aresriot.aws-rclusterprod-mes1-1.ext1-gp-bahrain-awsedge-1", "Bahrain"}, {"aresriot.aws-rclusterprod-mes1-1.tournament-gp-bahrain-awsedge-1", "Bahrain"},
        {"aresriot.aws-rclusterprod-bog1-1.latam-gp-bogota-1", "Bogotá"}, {"aresriot.aws-rclusterprod-bog1-1.tournament-gp-bogota-1", "Bogotá"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-1", "CMOB 1"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-2", "CMOB 2"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-3", "CMOB 3"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-cmob-4", "CMOB 4"}, {"aresriot.mtl-riot-ord2-3.ext1-gp-chicago-1", "Chicago"}, {"aresriot.mtl-riot-ord2-3.latam-gp-chicago-1", "Chicago"},
        {"aresqa.aws-rclusterprod-dfw1-1.dev1-gp-dallas-1", "Dallas"}, {"aresqa.aws-rclusterprod-euc1-1.dev1-gp-frankfurt-1", "Frankfurt"}, {"aresqa.aws-rclusterprod-euc1-1.stage1-gp-frankfurt-1", "Frankfurt"}, {"aresriot.aws-euc1-prod.eu-gp-frankfurt-1", "Frankfurt"}, {"aresriot.aws-euc1-prod.ext1-gp-eu1", "Frankfurt"}, {"aresriot.aws-euc1-prod.ext1-gp-frankfurt-1", "Frankfurt"}, {"aresriot.aws-rclusterprod-euc1-1.ext1-gp-eu1", "Frankfurt"},
        {"aresriot.aws-rclusterprod-euc1-1.tournament-gp-frankfurt-1", "Frankfurt"}, {"aresriot.aws-rclusterprod-euc1-1.eu-gp-frankfurt-1", "Frankfurt 1"}, {"aresriot.aws-rclusterprod-euc1-1.eu-gp-frankfurt-awsedge-1", "Frankfurt 2"}, {"aresriot.aws-ape1-prod.ap-gp-hongkong-1", "Hong Kong"}, {"aresriot.aws-ape1-prod.ext1-gp-hongkong-1", "Hong Kong"}, {"aresriot.aws-rclusterprod-ape1-1.ext1-gp-hongkong-1", "Hong Kong"}, {"aresriot.aws-rclusterprod-ape1-1.tournament-gp-hongkong-1", "Hong Kong"},
        {"aresriot.aws-rclusterprod-ape1-1.ap-gp-hongkong-1", "Hong Kong 1"}, {"aresriot.aws-rclusterprod-ape1-1.ap-gp-hongkong-awsedge-1", "Hong Kong 2"}, {"aresriot.mtl-riot-ist1-2.eu-gp-istanbul-1", "Istanbul"}, {"aresriot.mtl-riot-ist1-2.tournament-gp-istanbul-1", "Istanbul"}, {"aresriot.aws-euw2-prod.eu-gp-london-1", "London"}, {"aresriot.aws-rclusterprod-euw2-1.eu-gp-london-awsedge-1", "London"}, {"aresriot.aws-rclusterprod-euw2-1.tournament-gp-london-awsedge-1", "London"},
        {"aresriot.aws-rclusterprod-mad1-1.eu-gp-madrid-1", "Madrid"}, {"aresriot.aws-rclusterprod-mad1-1.tournament-gp-madrid-1", "Madrid"}, {"aresriot.mtl-tmx-mex1-1.ext1-gp-mexicocity-1", "Mexico City"}, {"aresriot.mtl-tmx-mex1-1.latam-gp-mexicocity-1", "Mexico City"}, {"aresriot.mtl-tmx-mex1-1.tournament-gp-mexicocity-1", "Mexico City"}, {"aresriot.mia1.latam-gp-miami-1", "Miami"}, {"aresriot.mia1.tournament-gp-miami-1", "Miami"}, {"aresriot.aws-aps1-prod.ap-gp-mumbai-1", "Mumbai"},
        {"aresriot.aws-rclusterprod-aps1-1.ap-gp-mumbai-awsedge-1", "Mumbai"}, {"aresriot.aws-rclusterprod-aps1-1.tournament-gp-mumbai-awsedge-1", "Mumbai"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-1", "Offline 1"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-2", "Offline 2"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-3", "Offline 3"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-4", "Offline 4"},
        {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-5", "Offline 5"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-6", "Offline 6"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-7", "Offline 7"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-offline-8", "Offline 8"}, {"aresriot.aws-euw3-prod.eu-gp-paris-1", "Paris"}, {"aresriot.aws-rclusterprod-euw3-1.tournament-gp-paris-1", "Paris"}, {"aresriot.aws-rclusterprod-euw3-1.eu-gp-paris-1", "Paris 1"},
        {"aresriot.aws-rclusterprod-euw3-1.eu-gp-paris-awsedge-1", "Paris 2"}, {"aresriot.mtl-ctl-scl2-2.ext1-gp-santiago-1", "Santiago"}, {"aresriot.mtl-ctl-scl2-2.latam-gp-santiago-1", "Santiago"}, {"aresriot.mtl-ctl-scl2-2.tournament-gp-santiago-1", "Santiago"}, {"aresriot.aws-rclusterprod-sae1-1.ext1-gp-saopaulo-1", "Sao Paulo"}, {"aresriot.aws-rclusterprod-sae1-1.tournament-gp-saopaulo-1", "Sao Paulo"}, {"aresriot.aws-sae1-prod.br-gp-saopaulo-1", "Sao Paulo"},
        {"aresriot.aws-sae1-prod.ext1-gp-saopaulo-1", "Sao Paulo"}, {"aresriot.aws-rclusterprod-sae1-1.br-gp-saopaulo-1", "Sao Paulo 1"}, {"aresriot.aws-rclusterprod-sae1-1.br-gp-saopaulo-awsedge-1", "Sao Paulo 2"}, {"aresriot.aws-apne2-prod.ext1-gp-seoul-1", "Seoul"}, {"aresriot.aws-apne2-prod.kr-gp-seoul-1", "Seoul"}, {"aresriot.aws-rclusterprod-apne2-1.ext1-gp-seoul-1", "Seoul"}, {"aresriot.aws-rclusterprod-apne2-1.tournament-gp-seoul-1", "Seoul"},
        {"aresriot.aws-rclusterprod-apne2-1.kr-gp-seoul-1", "Seoul 1"}, {"aresriot.aws-apse1-prod.ap-gp-singapore-1", "Singapore"}, {"aresriot.aws-apse1-prod.ext1-gp-singapore-1", "Singapore"}, {"aresriot.aws-rclusterprod-apse1-1.ext1-gp-singapore-1", "Singapore"}, {"aresriot.aws-rclusterprod-apse1-1.tournament-gp-singapore-1", "Singapore"}, {"aresriot.aws-rclusterprod-apse1-1.ap-gp-singapore-1", "Singapore 1"}, {"aresriot.aws-rclusterprod-apse1-1.ap-gp-singapore-awsedge-1", "Singapore 2"},
        {"aresriot.aws-eun1-prod.eu-gp-stockholm-1", "Stockholm"}, {"aresriot.aws-rclusterprod-eun1-1.tournament-gp-stockholm-1", "Stockholm"}, {"aresriot.aws-rclusterprod-eun1-1.eu-gp-stockholm-1", "Stockholm 1"}, {"aresriot.aws-rclusterprod-eun1-1.eu-gp-stockholm-awsedge-1", "Stockholm 2"}, {"aresriot.aws-apse2-prod.ap-gp-sydney-1", "Sydney"}, {"aresriot.aws-apse2-prod.ext1-gp-sydney-1", "Sydney"}, {"aresriot.aws-rclusterprod-apse2-1.ext1-gp-sydney-1", "Sydney"},
        {"aresriot.aws-rclusterprod-apse2-1.tournament-gp-sydney-1", "Sydney"}, {"aresriot.aws-rclusterprod-apse2-1.ap-gp-sydney-1", "Sydney 1"}, {"aresriot.aws-rclusterprod-apse2-1.ap-gp-sydney-awsedge-1", "Sydney 2"}, {"aresriot.aws-apne1-prod.ap-gp-tokyo-1", "Tokyo"}, {"aresriot.aws-apne1-prod.eu-gp-tokyo-1", "Tokyo"}, {"aresriot.aws-apne1-prod.ext1-gp-kr1", "Tokyo"}, {"aresriot.aws-apne1-prod.ext1-gp-tokyo-1", "Tokyo"}, {"aresriot.aws-rclusterprod-apne1-1.eu-gp-tokyo-1", "Tokyo"},
        {"aresriot.aws-rclusterprod-apne1-1.ext1-gp-kr1", "Tokyo"}, {"aresriot.aws-rclusterprod-apne1-1.tournament-gp-tokyo-1", "Tokyo"}, {"aresriot.aws-rclusterprod-apne1-1.ap-gp-tokyo-1", "Tokyo 1"}, {"aresriot.aws-rclusterprod-apne1-1.ap-gp-tokyo-awsedge-1", "Tokyo 2"}, {"aresqa.aws-usw2-dev.main1-gp-tournament-2", "Tournament"}, {"aresriot.aws-rclusterprod-atl1-1.na-gp-atlanta-1", "US Central (Georgia)"}, {"aresriot.aws-rclusterprod-atl1-1.tournament-gp-atlanta-1", "US Central (Georgia)"},
        {"aresriot.mtl-riot-ord2-3.na-gp-chicago-1", "US Central (Illinois)"}, {"aresriot.mtl-riot-ord2-3.tournament-gp-chicago-1", "US Central (Illinois)"}, {"aresriot.aws-rclusterprod-dfw1-1.na-gp-dallas-1", "US Central (Texas)"}, {"aresriot.aws-rclusterprod-dfw1-1.tournament-gp-dallas-1", "US Central (Texas)"}, {"aresriot.aws-rclusterprod-use1-1.na-gp-ashburn-1", "US East (N. Virginia 1)"}, {"aresriot.aws-rclusterprod-use1-1.na-gp-ashburn-awsedge-1", "US East (N. Virginia 2)"},
        {"aresriot.aws-rclusterprod-use1-1.ext1-gp-ashburn-1", "US East (N. Virginia)"}, {"aresriot.aws-rclusterprod-use1-1.pbe-gp-ashburn-1", "US East (N. Virginia)"}, {"aresriot.aws-rclusterprod-use1-1.tournament-gp-ashburn-1", "US East (N. Virginia)"}, {"aresriot.aws-use1-prod.ext1-gp-ashburn-1", "US East (N. Virginia)"}, {"aresriot.aws-use1-prod.na-gp-ashburn-1", "US East (N. Virginia)"}, {"aresriot.aws-use1-prod.pbe-gp-ashburn-1", "US East (N. Virginia)"},
        {"aresriot.aws-rclusterprod-usw1-1.na-gp-norcal-1", "US West (N. California 1)"}, {"aresriot.aws-rclusterprod-usw1-1.na-gp-norcal-awsedge-1", "US West (N. California 2)"}, {"aresriot.aws-rclusterprod-usw1-1.ext1-gp-na2", "US West (N. California)"}, {"aresriot.aws-rclusterprod-usw1-1.pbe-gp-norcal-1", "US West (N. California)"}, {"aresriot.aws-rclusterprod-usw1-1.tournament-gp-norcal-1", "US West (N. California)"}, {"aresriot.aws-usw1-prod.ext1-gp-na2", "US West (N. California)"},
        {"aresriot.aws-usw1-prod.ext1-gp-norcal-1", "US West (N. California)"}, {"aresriot.aws-usw1-prod.na-gp-norcal-1", "US West (N. California)"}, {"aresriot.aws-rclusterprod-usw2-1.na-gp-oregon-1", "US West (Oregon 1)"}, {"aresriot.aws-rclusterprod-usw2-1.na-gp-oregon-awsedge-1", "US West (Oregon 2)"}, {"aresriot.aws-rclusterprod-usw2-1.pbe-gp-oregon-1", "US West (Oregon)"}, {"aresriot.aws-rclusterprod-usw2-1.tournament-gp-oregon-1", "US West (Oregon)"},
        {"aresriot.aws-usw2-prod.na-gp-oregon-1", "US West (Oregon)"}, {"aresriot.aws-usw2-prod.pbe-gp-oregon-1", "US West (Oregon)"}, {"aresqa.aws-usw2-dev.main1-gp-1", "US West 1"}, {"aresqa.aws-usw2-dev.stage1-gp-1", "US West 1"}, {"aresqa.aws-usw2-dev.main1-gp-4", "US West 2"}, {"aresriot.aws-rclusterprod-waw1-1.eu-gp-warsaw-1", "Warsaw"}, {"aresriot.aws-rclusterprod-waw1-1.tournament-gp-warsaw-1", "Warsaw"}
    };

    internal static readonly Dictionary<string, string> AgentIdToAgent = new()
    {
        { "41fb69c1-4189-7b37-f117-bcaf1e96f1bf", "Astra" },
        { "5f8d3a7f-467b-97f3-062c-13acf203c006", "Breach" },
        { "9f0d8ba9-4140-b941-57d3-a7ad57c6b417", "Brimstone" },
        { "22697a3d-45bf-8dd7-4fec-84a9e28c69d7", "Chamber" },
        { "117ed9e3-49f3-6512-3ccf-0cada7e3823b", "Cypher" },
        { "cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Deadlock" },
        { "dade69b4-4f5a-8528-247b-219e5a1facd6", "Fade" },
        { "e370fa57-4757-3604-3648-499e1f642d3f", "Gekko" },
        { "95b78ed7-4637-86d9-7e41-71ba8c293152", "Harbor" },
        { "0e38b510-41a8-5780-5e8f-568b2a4f2d6c", "ISO" },
        { "add6443a-41bd-e414-f6ad-e58d267f4e95", "Jett" },
        { "1e58de9c-4950-5125-93e9-a0aee9f98746", "Killjoy" },
        { "bb2a4828-46eb-8cd1-e765-15848195d751", "Neon" },
        { "8e253930-4c05-31dd-1b6c-968525494517", "Omen" },
        { "eb93336a-449b-9c1b-0a54-a891f7921d69", "Phoenix" },
        { "f94c3b30-42be-e959-889c-5aa313dba261", "Raze" },
        { "a3bfb853-43b2-7238-a4f1-ad90e9e46bcc", "Reyna" },
        { "569fdd95-4d10-43ab-ca70-79becc718b46", "Sage" },
        { "6f2a04ca-43e0-be17-7f36-b3908627744d", "Skye" },
        { "320b2a48-4d9b-a075-30f1-1f93a9b638fa", "Sova" },
        { "707eab51-4836-f488-046a-cda6bf494859", "Viper" },
        { "7f94d92c-4234-0a36-9646-3a87eb8b5c89", "Yoru" },
        { "601dbbe7-43ce-be57-2a40-4abd24953621", "KAY/O" }
    };
}
