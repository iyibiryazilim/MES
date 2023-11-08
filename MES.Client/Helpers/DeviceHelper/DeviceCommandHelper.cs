using GoogleGson;
using MES.Client.Helpers.MESAPIHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Client.Helpers.DeviceHelper;

public class DeviceCommandHelper
{
    private readonly HttpClient _httpClient;
    
    public DeviceCommandHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendCommandAsync(string command, string baseUrl)
    {
        try
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
            
            var body = $"{{\"cmd\": \"{command}\"}}";
            StringContent stringContent = new StringContent(body);
            stringContent.Headers.Remove("Content-Type");
            stringContent.Headers.Add("Content-Type", "application/json");

            var response = await _httpClient.PostAsync("json", stringContent);
        } catch(Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    
    /*
    public async Task ConnectDeviceAsync()
    {
        try
        {

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://192.168.1.7:32000");

            var body = "{\"cmd\": \"connectDevice\"}";
            StringContent stringContent = new StringContent(body);
            stringContent.Headers.Remove("Content-Type");
            stringContent.Headers.Add("Content-Type", "application/json");

            var response = await httpClient.PostAsync("json", stringContent);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "tamam");
        }
    }

    public async Task InitDeviceAsync()
    {
        try
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://192.168.1.7:32000");

            var body = "{\"cmd\": \"initDevice\"}";
            StringContent stringContent = new StringContent(body);
            stringContent.Headers.Remove("Content-Type");
            stringContent.Headers.Add("Content-Type", "application/json");

            var response = await httpClient.PostAsync("json", stringContent);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "tamam");
        }
    }

    public async Task StartDeviceAsync()
    {
        try
        {

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://192.168.1.7:32000");

            var body = "{\"cmd\": \"startDevice\"}";
            StringContent stringContent = new StringContent(body);
            stringContent.Headers.Remove("Content-Type");
            stringContent.Headers.Add("Content-Type", "application/json");

            var response = await httpClient.PostAsync("json", stringContent);
           
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error :", ex.Message, "tamam");
        }
    }
    */

}
