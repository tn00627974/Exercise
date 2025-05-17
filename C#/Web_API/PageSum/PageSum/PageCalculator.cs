using System;
using System.Collections.Generic;

public static class PageCalculator
{
    public static int CountPages(string range)
    {
        if (string.IsNullOrWhiteSpace(range))
            return 0;

        var parts = range.Split(',');
        int count = 0;

        foreach (var part in parts)
        {
            string trimmed = part.Trim();

            if (trimmed.Contains('-'))
            {
                var bounds = trimmed.Split('-');
                if (bounds.Length != 2 ||
                    !int.TryParse(bounds[0], out int start) ||
                    !int.TryParse(bounds[1], out int end))
                {
                    throw new ArgumentException($"格式錯誤：'{trimmed}' 不是有效的範圍（例如 3-5）");
                }

                if (start > end)
                {
                    throw new ArgumentException($"區間錯誤：'{trimmed}' 起始頁（{start}）大於結束頁（{end}）");
                }

                count += end - start + 1;
            }
            else
            {
                if (!int.TryParse(trimmed, out int singlePage))
                {
                    throw new ArgumentException($"無效的頁碼：'{trimmed}' 不是整數");
                }

                count += 1;
            }
        }

        return count;
    }
}