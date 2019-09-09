using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Commands.Owner
{
    public partial class OwnerModule
    {

        [Command("Restart"), Alias("Reset"), Summary("Restarts the bot"),
        RequireOwner]
        public async Task RestartAsync(params string[] args)
        {
            int hours = 0, 
                minutes = 0, 
                seconds = 5;

            switch (args.Length)
            {
                case 1:
                    {
                        try
                        {
                            //Converts the argument string to seconds
                            seconds = Convert.ToInt32(args[0]);

                            while (seconds / 60 > 0)
                            {
                                seconds -= 60;
                                minutes += 1;
                            }

                            while (minutes / 60 > 0)
                            {
                                minutes -= 60;
                                hours += 1;
                            }
                        }
                        catch (Exception)
                        {
                            //Defaults to 5 seconds if conversion fails
                            seconds = 5;
                        }
                    }
                    break;
                case 2:
                    {
                        try
                        {
                            //Converts the argument string to minutes and seconds
                            minutes = Convert.ToInt32(args[0]);
                            seconds = Convert.ToInt32(args[1]);

                            while (seconds / 60 > 0)
                            {
                                seconds -= 60;
                                minutes += 1;
                            }

                            while (minutes / 60 > 0)
                            {
                                minutes -= 60;
                                hours += 1;
                            }
                        }
                        catch(Exception)
                        {
                            //Defaults to 5 seconds if conversion fails
                            minutes = 0;
                            seconds = 5;
                        }
                    }
                    break;
                case 3:
                    {
                        try
                        {
                            //Converts the argument string to hours, minutes and seconds
                            hours = Convert.ToInt32(args[0]);
                            minutes = Convert.ToInt32(args[1]);
                            seconds = Convert.ToInt32(args[2]);

                            while (seconds / 60 > 0)
                            {
                                seconds -= 60;
                                minutes += 1;
                            }

                            while (minutes / 60 > 0)
                            {
                                minutes -= 60;
                                hours += 1;
                            }

                        }
                        catch (Exception)
                        {
                            //Defaults to 5 seconds if conversion fails
                            hours = 0;
                            minutes = 0;
                            seconds = 5;
                        }
                    }
                    break;
                default:
                    break;
            }

            //Defaults to 5 seconds if no parameters are inputted or conversion fails
            var delay = new TimeSpan(hours, minutes, seconds);

            await Context.Client.StopAsync();
            await Task.Delay(delay);
            await Context.Client.StartAsync();
        }
    }
}
