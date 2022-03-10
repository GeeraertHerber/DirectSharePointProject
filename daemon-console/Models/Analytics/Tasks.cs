using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace daemon_console.Models.Analytics
{
    public class Tasks
    {
        [JsonProperty("entityRecognitionTasks")]
        public List<EntityRecognitionTask> EntityRecognitionTasks { get; set; }

        [JsonProperty("entityLinkingTasks")]
        public List<EntityLinkingTask> EntityLinkingTasks { get; set; }

        [JsonProperty("keyPhraseExtractionTasks")]
        public List<KeyPhraseExtractionTask> KeyPhraseExtractionTasks { get; set; }

        [JsonProperty("entityRecognitionPiiTasks")]
        public List<EntityRecognitionPiiTask> EntityRecognitionPiiTasks { get; set; }

        [JsonProperty("SentimentAnalysisTasks")]
        public List<SentimentAnalysisTask> SentimentAnalysisTasks { get; set; }

        [JsonProperty("extractiveSummarizationTasks")]
        public List<ExtractiveSummarizationTask> ExtractiveSummarizationTasks { get; set; }

        [JsonProperty("customEntityRecognitionTasks")]
        public List<CustomEntityRecognitionTask> CustomEntityRecognitionTasks { get; set; }

        [JsonProperty("customSingleClassificationTasks")]
        public List<CustomSingleClassificationTask> CustomSingleClassificationTasks { get; set; }

        [JsonProperty("customMultiClassificationTasks")]
        public List<CustomMultiClassificationTask> CustomMultiClassificationTasks { get; set; }
    }
}
