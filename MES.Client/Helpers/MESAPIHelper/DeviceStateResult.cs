using System;
namespace MES.Client.Helpers.MESAPIHelper;

public class DeviceStateResult
{
    public List<List<int>> ain { get; set; }
    public string cmd { get; set; }
    public string datetime { get; set; }
    public List<int> din { get; set; }
    public List<List<int>> encoder { get; set; }
    public List<int> relay { get; set; }
    public int type { get; set; }
}

