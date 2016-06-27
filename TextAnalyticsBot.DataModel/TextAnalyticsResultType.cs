﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyticsBot.DataModel
{
    public enum TextAnalyticsResultType
    {
        Sentiment, //Is text positive or negative?
        KeyPhrases, //What are people discussing in a single article?
        Topics, //What are people discussing across many articles?
        Languages //What language is text written in?
    }
}
