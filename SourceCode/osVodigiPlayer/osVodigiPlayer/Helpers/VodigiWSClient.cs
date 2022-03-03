using osVodigiPlayer.osVodigiWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osVodigiPlayer.Helpers
{
    public class VodigiWSClient
    {
        private osVodigiServiceSoapClient ws;
        public VodigiWSClient() : this(new Uri(PlayerConfiguration.configVodigiWebserviceURL)) { }

        public VodigiWSClient(Uri webserviceUri)
        {
            ws = new osVodigiServiceSoapClient(osVodigiServiceSoapClient.EndpointConfiguration.osVodigiServiceSoap12);
            ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(webserviceUri);
        }

        //
        // Resume:
        //     Represents the method that will handle various routed events that do not have
        //     specific event data beyond the data that is common for all routed events.
        //
        // Parameters:
        //   sender:
        //     The object where the event handler is attached.
        //
        //   e:
        //     The event data.
        public async Task<string> GetMediaDescriptorAsync()
        {
            Player_GetMediaToDownloadResponse resp = await ws.Player_GetMediaToDownloadAsync(PlayerConfiguration.configAccountID);
            return resp.Body.Player_GetMediaToDownloadResult;
        }

        public async Task<string> GetCurrentScheduleAsync()
        {
            Player_GetCurrentScheduleResponse resp = await ws.Player_GetCurrentScheduleAsync(PlayerConfiguration.configPlayerID);
            return resp.Body.Player_GetCurrentScheduleResult;
        }


        public async Task LogActivityAsync()
        {
            var response = await ws.ActivityLog_CreateAsync(PlayerConfiguration.configAccountID, 0, "Pla.yer", "Heartbeat", DateTime.UtcNow,
                   "Reported heartbeat player '" + PlayerConfiguration.configPlayerName + "' - ID: " + PlayerConfiguration.configPlayerID.ToString());

        }

        public async Task LogScreenStartAsync(string details)
        {
            var response = await ws.PlayerScreenLog_CreateAsync(PlayerConfiguration.configAccountID,
                                PlayerConfiguration.configPlayerID,
                                PlayerConfiguration.configPlayerName,
                                CurrentScreen.ScreenInfo.ScreenID,
                                CurrentScreen.ScreenInfo.ScreenName,
                                DateTime.UtcNow,
                                DateTime.UtcNow,
                                details);
        }


        public async Task LogScreenCloseAsync(int logID, DateTime closeDateTime)
        {
            await ws.PlayerScreenLog_UpdateCloseDateTimeAsync(logID, closeDateTime);
        }

        public async Task<int> LogScreenContentStartAsync(int screenID, string screenName, int screenContentID, string screenContentName, int screenContentTypeID, string screenContentTypeName)
        {
             var response = await ws.PlayerScreenContentLog_CreateAsync(PlayerConfiguration.configAccountID,
                                            PlayerConfiguration.configPlayerID,
                                            PlayerConfiguration.configPlayerName, 
                                            screenID,
                                            screenName,
                                            screenContentID,
                                            screenContentName,
                                            screenContentTypeID,
                                            screenContentTypeName,
                                            DateTime.UtcNow,
                                            DateTime.UtcNow,
                                            String.Empty);

            return response.Body.PlayerScreenContentLog_CreateResult;

        }

        public async Task LogScreenContentCloseAsync(int logID, DateTime closeDateTime)
        {
            await ws.PlayerScreenContentLog_UpdateCloseDateTimeAsync(logID, closeDateTime);
        }

        public async Task<string> GetDatabaseVersionAsync()
        {
            DatabaseVersion_GetResponse respo = await ws.DatabaseVersion_GetAsync();
            return respo.Body.DatabaseVersion_GetResult.Version;
        }

        public async Task<osVodigiPlayer.Data.Account> GetAccountByNameAsync(string accountName)
        {
            Account_GetByNameResponse respo = await ws.Account_GetByNameAsync(accountName);

            osVodigiWS.Account acc = respo.Body.Account_GetByNameResult;

            return new osVodigiPlayer.Data.Account
            {
                AccountID = acc.AccountID,
                AccountName = acc.AccountName
            };
        }

        public async Task<osVodigiPlayer.Data.Player> GetPlayerByNameAsync(int accountID, string playerName)
        {
            Player_GetByNameResponse respo = await ws.Player_GetByNameAsync(accountID, playerName);
            osVodigiWS.Player pl = respo.Body.Player_GetByNameResult;
            return new osVodigiPlayer.Data.Player
            {
                PlayerID = pl.PlayerID,
                PlayerGroupID = pl.PlayerGroupID,
                PlayerName = pl.PlayerName
            };
        }

        public async Task<int> CreateAnsweredSurveyIDAsync(int surveyID)
        {
            int ans = await ws.AnsweredSurvey_CreateAsync(PlayerConfiguration.configAccountID, surveyID, PlayerConfiguration.configPlayerID);

            return ans;
        }

        public async Task<int> CreateAnsweredSurveyQuestionOptionAsync(int iAnsweredSurveyID, int iSurveyQuestionOptionID, bool bIsSelected)
        {
            int response = await ws.AnsweredSurveyQuestionOption_CreateAsync(iAnsweredSurveyID, iSurveyQuestionOptionID, bIsSelected);
            return await Task.FromResult(response);
        }
    }
}
