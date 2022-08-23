public async Task ModulePingsBroker(string moduleName, string containerId)
        {
            try
            {
                Module module = new Module($"{moduleName}");
                string password;
                
                string command = "docker exec " + containerId + " env cat /run/secrets/" + moduleName;
                command = command.Replace("\"", "\\\"");
                logger.LogInformation($"command: {command}");

                IProcessProvider p;
                p = factory?.Create(new ProcessStartInfo
                {
                    FileName = "/usr/bin/docker",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    ArgumentList = { "exec", { containerId }, "env", "cat", "/run/secrets", { moduleName} }
                });
                password = await p.ReadLine().LastAsync();
                logger.LogInformation($"password: {password}");
           
                await protocol.Ping(moduleName, password);
            }
            catch (TimeoutException e)
            {
                logger.LogErrorLine($"Ping timed out. {moduleName} is not connected to the broker. => {e.Message} {e.StackTrace}");
            }
            catch (Exception e)
            {
                logger.LogErrorLine($"Exception while {moduleName} pinged broker. => {e.Message} {e.StackTrace}");
            }
        }
