﻿using System.Buffers.Binary;
using System.Globalization;
using System.Text.Json;

namespace TaikoLocalServer.Controllers.Game;

[Route("/v12r03/chassis/playresult.php")]
[ApiController]
public class PlayResultController : BaseController<PlayResultController>
{
    private readonly TaikoDbContext context;

    public PlayResultController(TaikoDbContext context)
    {
        this.context = context;
    }

    [HttpPost]
    [Produces("application/protobuf")]
    public IActionResult UploadPlayResult([FromBody] PlayResultRequest request)
    {
        Logger.LogInformation("PlayResult request : {Request}", request.Stringify());
        var decompressed = GZipBytesUtil.DecompressGZipBytes(request.PlayresultData);

        var playResultData = Serializer.Deserialize<PlayResultDataRequest>(new ReadOnlySpan<byte>(decompressed));

        Logger.LogInformation("Play result data {Data}", playResultData.Stringify());

        var response = new PlayResultResponse
        {
            Result = 1
        };   
       
        var lastPlayDatetime = DateTime.ParseExact(playResultData.PlayDatetime, Constants.DATE_TIME_FORMAT, CultureInfo.InvariantCulture);
        
        UpdateUserData(request, playResultData, lastPlayDatetime);
        var playMode = (PlayMode)playResultData.PlayMode;
        
        if (playMode == PlayMode.DanMode)
        {
            UpdateDanPlayData(request, playResultData);
            context.SaveChanges();
            return Ok(response);
        }
        
        var bestData = context.SongBestData.Where(datum => datum.Baid == request.BaidConf).ToList();
        for (var songNumber = 0; songNumber < playResultData.AryStageInfoes.Count; songNumber++)
        {
            var stageData = playResultData.AryStageInfoes[songNumber];
            
            UpdateBestData(request, stageData, bestData);

            UpdatePlayData(request, songNumber, stageData, lastPlayDatetime);
        }

        context.SaveChanges();
        return Ok(response);
    }

    private void UpdateDanPlayData(PlayResultRequest request, PlayResultDataRequest playResultDataRequest)
    {
        if (playResultDataRequest.IsNotRecordedDan)
        {
            Logger.LogInformation("Dan score will not be saved!");
            return;
        }
        var danScoreDataQuery = context.DanScoreData
             .Where(datum => datum.Baid == request.BaidConf &&
                             datum.DanId == playResultDataRequest.DanId)
             .Include(datum => datum.DanStageScoreData);
        var danScoreData = new DanScoreDatum
        {
            Baid = request.BaidConf,
            DanId = playResultDataRequest.DanId
        };
        var insert = true;
        if (danScoreDataQuery.Any())
        {
            danScoreData = danScoreDataQuery.First();
            insert = false;
        }
        danScoreData.ClearState = (DanClearState)Math.Max(playResultDataRequest.DanResult, (uint)danScoreData.ClearState);
        danScoreData.ArrivalSongCount = Math.Max((uint)playResultDataRequest.AryStageInfoes.Count, danScoreData.ArrivalSongCount);
        danScoreData.ComboCountTotal = Math.Max(playResultDataRequest.ComboCntTotal, danScoreData.ComboCountTotal);
        danScoreData.SoulGaugeTotal = Math.Max(playResultDataRequest.SoulGaugeTotal, danScoreData.SoulGaugeTotal);

        UpdateDanStageData(playResultDataRequest, danScoreData);

        if (insert)
        {
            context.DanScoreData.Add(danScoreData);
            return;
        }
        context.DanScoreData.Update(danScoreData);
    }
    private void UpdateDanStageData(PlayResultDataRequest playResultDataRequest, DanScoreDatum danScoreData)
    {
        for (var songNumber = 0; songNumber < playResultDataRequest.AryStageInfoes.Count; songNumber++)
        {
            var stageData = playResultDataRequest.AryStageInfoes[songNumber];
            var add = true;

            var danStageData = new DanStageScoreDatum
            {
                Baid = danScoreData.Baid,
                DanId = danScoreData.DanId,
                SongNumber = (uint)songNumber
            };
            if (danScoreData.DanStageScoreData.Any(datum => datum.SongNumber == songNumber))
            {
                danStageData = danScoreData.DanStageScoreData.First(datum => datum.SongNumber == songNumber);
                add = false;
            }

            danStageData.HighScore = Math.Max(danStageData.HighScore, stageData.PlayScore);
            danStageData.ComboCount = Math.Max(danStageData.ComboCount, stageData.ComboCnt);
            danStageData.DrumrollCount = Math.Max(danStageData.DrumrollCount, stageData.PoundCnt);
            danStageData.GoodCount = Math.Max(danStageData.GoodCount, stageData.GoodCnt);
            danStageData.TotalHitCount = Math.Max(danStageData.TotalHitCount, stageData.HitCnt);
            danStageData.OkCount = Math.Min(danStageData.OkCount, stageData.OkCnt);
            danStageData.BadCount = Math.Min(danStageData.BadCount, stageData.NgCnt);

            if (add)
            {
                context.DanStageScoreData.Add(danStageData);
                continue;
            }

            context.DanStageScoreData.Update(danStageData);
        }
    }

    private void UpdatePlayData(PlayResultRequest request, int songNumber, PlayResultDataRequest.StageData stageData, DateTime lastPlayDatetime)
    {
        var playData = new SongPlayDatum
        {
            Baid = request.BaidConf,
            SongNumber = (uint)songNumber,
            GoodCount = stageData.GoodCnt,
            OkCount = stageData.OkCnt,
            MissCount = stageData.NgCnt,
            ComboCount = stageData.ComboCnt,
            HitCount = stageData.HitCnt,
            Crown = PlayResultToCrown(stageData),
            Score = stageData.PlayScore,
            ScoreRate = stageData.ScoreRate,
            ScoreRank = (ScoreRank)stageData.ScoreRank,
            Skipped = stageData.IsSkipUse,
            SongId = stageData.SongNo,
            PlayTime = lastPlayDatetime,
            Difficulty = (Difficulty)stageData.Level
        };
        context.SongPlayData.Add(playData);
    }
    private void UpdateUserData(PlayResultRequest request, PlayResultDataRequest playResultData, DateTime lastPlayDatetime)
    {
        var userdata = new UserDatum
        {
            Baid = request.BaidConf
        };
        if (context.UserData.Any(datum => datum.Baid == request.BaidConf))
        {
            userdata = context.UserData.First(datum => datum.Baid == request.BaidConf);
        }

        userdata.Title = playResultData.Title;
        userdata.TitlePlateId = playResultData.TitleplateId;
        var costumeData = new List<uint>
        {
            playResultData.AryCurrentCostume.Costume1,
            playResultData.AryCurrentCostume.Costume2,
            playResultData.AryCurrentCostume.Costume3,
            playResultData.AryCurrentCostume.Costume4,
            playResultData.AryCurrentCostume.Costume5
        };
        userdata.CostumeData = JsonSerializer.Serialize(costumeData);

        var lastStage = playResultData.AryStageInfoes.Last();
        var option = BinaryPrimitives.ReadInt16LittleEndian(lastStage.OptionFlg);
        userdata.OptionSetting = option;
        userdata.IsSkipOn = lastStage.IsSkipOn;
        userdata.IsVoiceOn = lastStage.IsVoiceOn;
        userdata.NotesPosition = lastStage.NotesPosition;

        userdata.LastPlayDatetime = lastPlayDatetime;
        userdata.LastPlayMode = playResultData.PlayMode;
        context.UserData.Update(userdata);
    }

    private void UpdateBestData(PlayResultRequest request, PlayResultDataRequest.StageData stageData, IEnumerable<SongBestDatum> bestData)
    {
        var insert = false;
        var data = stageData;
        var bestDatum = bestData
            .FirstOrDefault(datum => datum.SongId == data.SongNo &&
                                     datum.Difficulty == (Difficulty)data.Level);

        // Determine whether it is dondaful crown as this is not reflected by play result
        var crown = PlayResultToCrown(stageData);

        if (bestDatum is null)
        {
            insert = true;
            bestDatum = new SongBestDatum
            {
                Baid = request.BaidConf,
                SongId = stageData.SongNo,
                Difficulty = (Difficulty)stageData.Level
            };
        }

        bestDatum.UpdateBestData(crown, stageData.ScoreRank, stageData.PlayScore, stageData.ScoreRate);

        if (insert)
        {
            context.SongBestData.Add(bestDatum);
        }
        else
        {
            context.SongBestData.Update(bestDatum);
        }
    }
    private static CrownType PlayResultToCrown(PlayResultDataRequest.StageData stageData)
    {
        var crown = (CrownType)stageData.PlayResult;
        if (crown == CrownType.Gold && stageData.OkCnt == 0)
        {
            crown = CrownType.Dondaful;
        }
        return crown;
    }
}