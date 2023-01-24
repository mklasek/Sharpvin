using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;


namespace Sharpvin
{
    internal class MessageHandler
    {
        //singleton instance
        private static MessageHandler instance;

        //fields
        private FixedStringQueue MsgQueue;
        private ulong selfID;
        private readonly CurrencyConverter Currency;
        private readonly MessageAnalysis Analysis;
        private readonly YouTubeSearcher Youtube;

        //constructor
        private MessageHandler()
        {
            MsgQueue = FixedStringQueue.Create(3);
            Currency = new CurrencyConverter();
            Analysis = new MessageAnalysis();
            Youtube = new YouTubeSearcher();
        }

        public void SetID(ulong id)
        {
            this.selfID = id;
        }

        public static MessageHandler GetInstance()
        {
            if (instance == null)
            {
                instance = new MessageHandler();
            }

            return instance;
        }

        public async void OnReady()
        {
            await Currency.UpdateRates();
        }


        public async Task OnMessageReceived(SocketMessage rawSM)
        {
            //ignore own messages and ignore Lenny commands
            if (rawSM.Author.Id == instance.selfID || rawSM.Content.StartsWith('?'))
                return;

            //convert to lowercase
            String msg = rawSM.Content.ToLower();

            //spam aid
            instance.MsgQueue.Push(msg);
            if (instance.MsgQueue.isRepeating())
            {
                await rawSM.Channel.SendMessageAsync(rawSM.Content);
            }

            (bool isCommand, bool isPolite, bool hasNumber) = Analysis.ScanMessage(msg);

            //autoconvert from czk to eur
            if ((msg.Contains("czk") || msg.Contains("peanut")) && !isCommand)
            {
                String[] words = msg.Split(' ');
                double value = 0;
                bool isNumeric = false;
                foreach (String word in words)
                {
                    String w = word.Replace('.', ',');
                    isNumeric = double.TryParse(w, out value);
                    if (isNumeric)
                        break;
                }

                if (isNumeric)
                {
                    double eur = Currency.Convert(value, "CZK", "EUR");
                    await rawSM.Channel.SendMessageAsync($"{value.ToString().Replace(',', '.')} peanuts is {eur.ToString().Replace(',', '.')} EUR");
                }

            }
            
            if (isCommand)
            {
                //convert currency
                if (msg.Contains("convert"))
                {
                    if (!isPolite)
                    {
                        await rawSM.Channel.SendMessageAsync("Do it yourself");
                        return;
                    }
                    else if (!hasNumber)
                    {
                        await rawSM.Channel.SendMessageAsync("I don't see a number to convert");
                        return;
                    }


                    double value = 0, final = 0;
                    String code_from = "", code_to = "";
                    try
                    {
                        (value, code_from, code_to) = Analysis.FindConversion(msg);
                        final = Currency.Convert(value, code_from, code_to);
                    }
                    catch (Exception ex)
                    {
                        await rawSM.Channel.SendMessageAsync(ex.Message);
                        return;
                    }

                    await rawSM.Channel.SendMessageAsync($"{value.ToString().Replace(',', '.')} {code_from} is {final.ToString().Replace(',', '.')} {code_to}");
                }


                //randomly generated answers
                else if (msg.Contains('?'))
                {
                    Random random = new Random();

                    if (msg.Contains("or"))
                    {
                        List<String> options = Analysis.FindOptions(msg);

                        int index = random.Next(options.Count);
                        await rawSM.Channel.SendMessageAsync(options[index]);
                    }

                    else
                    {
                        List<String> answers = new List<String>()
                        {
                            "yeah",
                            "yes",
                            "yup",
                            "nope",
                            "no",
                            "nah"
                        };

                        int index = random.Next(answers.Count);
                        await rawSM.Channel.SendMessageAsync(answers[index]);
                    }
                    
                }

                //fetching youtube videos
                else if (msg.Contains("youtube"))
                {
                    if (!isPolite)
                    {
                        await rawSM.Channel.SendMessageAsync("Do it yourself");
                        return;
                    }

                    String query = Analysis.FindQuery(msg);

                    if (query != "")
                    {
                        String video = await Youtube.Search(query);
                        await rawSM.Channel.SendMessageAsync(video);
                    }
                    else
                    {
                        await rawSM.Channel.SendMessageAsync("I didn't understand the request, sorry");
                    }
                }

                else if (msg.Contains("next video"))
                {
                    if (!isPolite)
                    {
                        await rawSM.Channel.SendMessageAsync("Do it yourself");
                        return;
                    }

                    String video = Youtube.NextVideo();
                    if (video != "")
                        await rawSM.Channel.SendMessageAsync(video);
                    else
                        await rawSM.Channel.SendMessageAsync("Nothing left to post");
                }
            }
        }

    }
}
