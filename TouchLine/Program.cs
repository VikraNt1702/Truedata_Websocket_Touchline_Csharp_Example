using System;
using TrueData_DotNet;
using Newtonsoft.Json;

namespace TouchLine
{
    //First Commit
    class Program
    {
        static TDWebSocket tDWebSocket;
        static void Main(string[] args) 
        {
            ConnectWebSocketRT();
        }
        private static void ConnectWebSocketRT()
        {
            tDWebSocket = new TDWebSocket("YourId", "YourPassword", "wss://push.truedata.in", 8094);
            
            tDWebSocket.OnConnect += TDWebSocket_OnConnect;
            tDWebSocket.OnDataArrive += TDWebSocket_OnDataArrive;
            tDWebSocket.OnClose += TDWebSocket_OnClose;
            tDWebSocket.OnError += TDWebSocket_OnError;

            tDWebSocket.ConnectAsync();

        }
        private static void TDWebSocket_OnError(object sender, EventErrorArgs e)
        {
            Console.WriteLine(e.ErrorMsg);
            Console.Read();
        }

        private static void TDWebSocket_OnClose(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected");
            Console.Read();
        }

        private static void TDWebSocket_OnDataArrive(object sender, EventDataArgs e)
        {

            try
            {
                
                if (e.JsonMsg.Contains("HeartBeat"))
                {
                    HeartBeat hb = new HeartBeat();
                    hb = JsonConvert.DeserializeObject<HeartBeat>(e.JsonMsg);
                    Console.WriteLine("Heartbeat " + hb.timestamp + "-" + hb.message);
                }
                else if (e.JsonMsg.Contains("symbolsadded"))
                {
                    ResPonse result = new();
                    List<SymbolData> datalist = new();

                    ResPonseVM res = JsonConvert.DeserializeObject<ResPonseVM>(e.JsonMsg);

                    foreach (var symbol in res.Symbollist)
                    {
                        var sd = new SymbolData
                        {
                            Symbol = symbol[0],
                            SymbolId = int.TryParse(symbol[1], out var symbolId) ? symbolId : 0,
                            Timestamp = DateTime.TryParse(symbol[2], out var timestamp) ? timestamp : DateTime.MinValue,
                            LTP = float.TryParse(symbol[3], out var ltp) ? ltp : 0f,
                            TickVolume = int.TryParse(symbol[4], out var tickVolume) ? tickVolume : 0,
                            ATP = float.TryParse(symbol[5], out var atp) ? atp : 0f,
                            ToatalVolume = int.TryParse(symbol[6], out var totalVolume) ? totalVolume : 0,
                            Open = float.TryParse(symbol[7], out var open) ? open : 0f,
                            High = float.TryParse(symbol[8], out var high) ? high : 0f,
                            Low = float.TryParse(symbol[9], out var low) ? low : 0f,
                            PrevClose = float.TryParse(symbol[10], out var prevClose) ? prevClose : 0f,
                            OI = int.TryParse(symbol[11], out var oi) ? oi : 0,
                            PreviousOpenInterestClose = int.TryParse(symbol[12], out var previousOiClose) ? previousOiClose : 0,
                            TurnOver = float.TryParse(symbol[13], out var turnover) ? turnover : 0f,
                            Bid = float.TryParse(symbol[14], out var bid) ? bid : 0f,
                            BidQty = int.TryParse(symbol[15], out var bidQty) ? bidQty : 0,
                            Ask = float.TryParse(symbol[16], out var ask) ? ask : 0f,
                            AskQty = int.TryParse(symbol[17], out var askQty) ? askQty : 0
                        };

                        datalist.Add(sd); //use the datalist as per your usage
                    }

                    result.Success = res.Success;
                    result.Message = res.Message;
                    result.Symbolsadded = res.Symbolsadded;

                }
                else if (e.JsonMsg.Contains("TrueData"))
                {
                    Console.WriteLine("Connected");
                    tDWebSocket.Send("{\"method\":\"touchline\"}");
                }
                else
                    Console.WriteLine(e.JsonMsg);
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Error: {ex.Message}");
            }
        }
        private static void TDWebSocket_OnConnect(object sender, EventArgs e)
        {
            
        }
    }
}
public class SymbolData
{
    [JsonIgnore]
    public string Symbol { get; set; }
    [JsonIgnore]
    public int SymbolId { get; set; }
    [JsonIgnore]
    public DateTime Timestamp { get; set; }
    [JsonIgnore]
    public float LTP { get; set; }
    [JsonIgnore]
    public int TickVolume { get; set; }
    [JsonIgnore]
    public float ATP { get; set; }
    [JsonIgnore]
    public int ToatalVolume { get; set; }
    public float Open { get; set; }
    public float High { get; set; }
    public float Low { get; set; }
    public float PrevClose { get; set; }
    public int OI { get; set; }
    public int PreviousOpenInterestClose { get; set; }
    public float TurnOver { get; set; }
    public float Bid { get; set; }
    public int BidQty { get; set; }
    public float Ask { get; set; }
    public int AskQty { get; set; }

    
}
public class ResPonseVM
{
    
    public bool Success { get; set; }
    public string Message { get; set; }
    public int Symbolsadded { get; set; }
    public List<List<string>> Symbollist { get; set; }
    public int Totalsymbolsubscribed { get; set; }
}
public class ResPonse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int Symbolsadded { get; set; }
    public List<List<SymbolData>> Symbollist { get; set; }
    public int Totalsymbolsubscribed { get; set; }
}