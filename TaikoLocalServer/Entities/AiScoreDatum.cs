﻿namespace TaikoLocalServer.Entities;

public class AiScoreDatum
{
    public uint Baid { get; set; }
    
    public uint SongId { get; set; }
    
    public Difficulty Difficulty { get; set; }
    
    public bool IsWin { get; set; }

    public List<AiSectionScoreDatum> AiSectionScoreData { get; set; } = new();
    
    public virtual Card? Ba { get; set; }
}