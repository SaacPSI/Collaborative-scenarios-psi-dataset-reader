using Microsoft.Psi.Audio;
using Microsoft.Psi.AzureKinect;
using Microsoft.Psi.Imaging;
using Microsoft.Psi.Speech;
using Microsoft.Psi;
using sample_project.specific_class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace sample_project
{
    class Receiver
    {
        public DateTime startTask = DateTime.MaxValue;
        public DateTime endTask = DateTime.MinValue;

        public List<StreamWriter> streamWriterList = new List<StreamWriter>();
        StreamWriter positionrotation1Writer;
        StreamWriter positionrotation2Writer;
        StreamWriter gazeEventWriter;
        StreamWriter taskInteractionEventWriter;
        
        string headWristHeadupEU = "utc_timestamp_ms,participant_id,position_x,position_y,position_z,rotation_x,rotation_y,rotation_z,left_hand_position_x,left_hand_position_y,left_hand_position_z,left_hand_rotation_x,left_hand_rotation_y,left_hand_rotation_z,right_hand_position_x,right_hand_position_y,right_hand_position_z,right_hand_rotation_x,right_hand_rotation_y,right_hand_rotation_z".Replace(',', ';');
        string gazeEventHeadupEU = "utc_timestamp_ms,participant_id,type,gaze_state,gazer_id,user_or_object_gazed_id".Replace(',', ';');
        string taskInteractionHeadupEU = "utc_timestamp_ms,participant_id,type,object_id".Replace(',', ';');
        /*StreamWriter gazeFilteredEventWriter;
        StreamWriter IndexesWriter;
        StreamWriter profileConfidenceWriter;*/
        /*string gazeEventFilteredHeadupEU = "utc_timestamp_ms,participant_id,type,utc_timestamp_start_gaze,utc_timestamp_end_gaze,gaze_duration_ms,gazer_id,user_or_object_gazed_id".Replace(',', ';');
        string indexesHeadupEU = "utc_timestamp_ms,verbal_participation_user_1_index,verbal_participation_user_2_index,task_participation_user_1_index,task_participation_user_2_index,two_gazed_one_index,one_gazed_two_index,speech_equality_index,task_equality_index,jva_index,turn_taking_with_overlap_index,turn_taking_without_overlap_index,overlap_index,talking_most_id,tasking_most_id,most_watched_id,leads_visual_attention_id,1_2_distanceobject_id,EN_confidence_value,SOL_confidence_value,SOC_confidence_value,TS_confidence_value,LF_confidence_value,TTNA_confidence_value,TTA_confidence_value".Replace(',', ';');
        string confidenceHeadupEU = "utc_timestamp_ms,EN_confidence_value,SOL_confidence_value,SOC_confidence_value,TS_confidence_value,LF_confidence_value,TTNA_confidence_value,TTA_confidence_value";*/
        bool isHeadup = false;

        private Tuple<Vector3, Vector3> currentLeftHand1 = new Tuple<Vector3, Vector3>(Vector3.Zero, Vector3.Zero);
        private Tuple<Vector3, Vector3> currentLeftHand2 = new Tuple<Vector3, Vector3>(Vector3.Zero, Vector3.Zero);
        private Tuple<Vector3, Vector3> currentRightHand1 = new Tuple<Vector3, Vector3>(Vector3.Zero, Vector3.Zero);
        private Tuple<Vector3, Vector3> currentRightHand2 = new Tuple<Vector3, Vector3>(Vector3.Zero, Vector3.Zero);
        static TakenAndPositionnedPieceData Tbloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "TBloc", false, false);
        static TakenAndPositionnedPieceData Lbloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "LBloc", false, false);
        static TakenAndPositionnedPieceData Sbloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "SBloc", false, false);
        static TakenAndPositionnedPieceData Zbloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "ZBloc", false, false);
        static TakenAndPositionnedPieceData Linebloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "LineBloc", false, false);
        static TakenAndPositionnedPieceData Cubebloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "CubeBloc", false, false);
        static TakenAndPositionnedPieceData Jbloc = new TakenAndPositionnedPieceData(DateTime.MinValue, DateTime.MinValue, "JBloc", false, false);
        public List<TakenAndPositionnedPieceData> positionnedPiece = new List<TakenAndPositionnedPieceData>() { Tbloc, Lbloc, Sbloc, Zbloc, Linebloc, Cubebloc, Jbloc };

        public Receiver(Pipeline pipeline, string path)
        {
            //Receivers
            this.Video1In = pipeline.CreateReceiver<Shared<EncodedImage>>(this, ReceiveVideo1, nameof(this.Video1In));
            this.Video2In = pipeline.CreateReceiver<Shared<EncodedImage>>(this, ReceiveVideo2, nameof(this.Video2In));

            this.bodiesIn = pipeline.CreateReceiver<List<AzureKinectBody>>(this, ReceiveBodiesKinect, nameof(this.bodiesIn));

            this.Head1In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveHead1, nameof(this.Head1In));
            this.Head2In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveHead2, nameof(this.Head2In));
            this.LeftHand1In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveLeftHand1, nameof(this.LeftHand1In));
            this.LeftHand2In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveLeftHand2, nameof(this.LeftHand2In));
            this.RightHand1In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveRightHand1, nameof(this.RightHand1In));
            this.RightHand2In = pipeline.CreateReceiver<Tuple<Vector3, Vector3>>(this, ReceiveRightHand2, nameof(this.RightHand2In));
            
            this.OtherEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveOtherEvent1, nameof(this.OtherEvent1In));
            this.GazeEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveGazeEvent1, nameof(this.GazeEvent1In));
            this.UngazeEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveUngazeEvent1, nameof(this.UngazeEvent1In));
            this.HoverEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveHoverEvent1, nameof(this.HoverEvent1In));
            this.UnhoverEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveUnhoverEvent1, nameof(this.UnhoverEvent1In));
            this.SelectEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveSelectEvent1, nameof(this.SelectEvent1In));
            this.UnselectEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveUnselectEvent1, nameof(this.UnselectEvent1In));
            this.ActivateEvent1In = pipeline.CreateReceiver<EventData>(this, ReceiveActivateEvent1, nameof(this.ActivateEvent1In));

            this.OtherEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveOtherEvent2, nameof(this.OtherEvent2In));
            this.GazeEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveGazeEvent2, nameof(this.GazeEvent2In));
            this.UngazeEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveUngazeEvent2, nameof(this.UngazeEvent2In));
            this.HoverEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveHoverEvent2, nameof(this.HoverEvent2In));
            this.UnhoverEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveUnhoverEvent2, nameof(this.UnhoverEvent2In));
            this.SelectEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveSelectEvent2, nameof(this.SelectEvent2In));
            this.UnselectEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveUnselectEvent2, nameof(this.UnselectEvent2In));
            this.ActivateEvent2In = pipeline.CreateReceiver<EventData>(this, ReceiveActivateEvent2, nameof(this.ActivateEvent2In));

            this.GazeAvatar1In = pipeline.CreateReceiver<EventData>(this, ReceiveGazeAvatarEvent1, nameof(this.GazeAvatar1In));
            this.ValidationPuzzle1In = pipeline.CreateReceiver<EventData>(this, ReceiveValidationPuzzleEvent1, nameof(this.ValidationPuzzle1In));
            this.GazeAvatar2In = pipeline.CreateReceiver<EventData>(this, ReceiveGazeAvatarEvent2, nameof(this.GazeAvatar2In));
            this.ValidationPuzzle2In = pipeline.CreateReceiver<EventData>(this, ReceiveValidationPuzzleEvent2, nameof(this.ValidationPuzzle2In));
            this.CollabEventIn = pipeline.CreateReceiver<CollabEventData>(this, ReceiveCollabEvent, nameof(this.CollabEventIn));

            this.vad1In = pipeline.CreateReceiver<bool>(this, ReceiveVad1Result, nameof(this.vad1In));
            this.vad2In = pipeline.CreateReceiver<bool>(this, ReceiveVad2Result, nameof(this.vad2In));
            this.stt1In = pipeline.CreateReceiver<IStreamingSpeechRecognitionResult>(this, ReceiveStt1Result, nameof(this.stt1In));
            this.stt2In = pipeline.CreateReceiver<IStreamingSpeechRecognitionResult>(this, ReceiveStt2Result, nameof(this.stt2In));

            this.timerIn = pipeline.CreateReceiver<TimeSpan>(this, ReceiveTimer, nameof(this.timerIn));

            // Raw data emitters initialization
            this.Video1Out = pipeline.CreateEmitter<Shared<EncodedImage>>(this, nameof(this.Video1Out));
            this.Video2Out = pipeline.CreateEmitter<Shared<EncodedImage>>(this, nameof(this.Video2Out));

            this.bodiesOut = pipeline.CreateEmitter<List<AzureKinectBody>>(this, nameof(this.bodiesOut));

            this.Head1Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.Head1Out));
            this.Head2Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.Head2Out));
            this.LeftHand1Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.LeftHand1Out));
            this.LeftHand2Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.LeftHand2Out));
            this.RightHand1Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.RightHand1Out));
            this.RightHand2Out = pipeline.CreateEmitter<Tuple<Vector3, Vector3>>(this, nameof(this.RightHand2Out));

            this.OtherEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.OtherEvent1Out));
            this.GazeEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.GazeEvent1Out));
            this.UngazeEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UngazeEvent1Out));
            this.HoverEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.HoverEvent1Out));
            this.UnhoverEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UnhoverEvent1Out));
            this.SelectEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.SelectEvent1Out));
            this.UnselectEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UnselectEvent1Out));
            this.ActivateEvent1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.ActivateEvent1Out));

            this.OtherEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.OtherEvent2Out));
            this.GazeEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.GazeEvent2Out));
            this.UngazeEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UngazeEvent2Out));
            this.HoverEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.HoverEvent2Out));
            this.UnhoverEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UnhoverEvent2Out));
            this.SelectEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.SelectEvent2Out));
            this.UnselectEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.UnselectEvent2Out));
            this.ActivateEvent2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.ActivateEvent2Out));

            this.GazeAvatar1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.GazeAvatar1Out));
            this.GazeAvatar2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.GazeAvatar2Out));
            this.ValidationPuzzle1Out = pipeline.CreateEmitter<EventData>(this, nameof(this.ValidationPuzzle1Out));
            this.ValidationPuzzle2Out = pipeline.CreateEmitter<EventData>(this, nameof(this.ValidationPuzzle2Out));
            this.CollabEventOut = pipeline.CreateEmitter<CollabEventData>(this, nameof(this.CollabEventOut));

            this.vad1Out = pipeline.CreateEmitter<bool>(this, nameof(this.vad1Out));
            this.vad2Out = pipeline.CreateEmitter<bool>(this, nameof(this.vad2Out));
            this.stt1Out = pipeline.CreateEmitter<IStreamingSpeechRecognitionResult>(this, nameof(this.stt1Out));
            this.stt2Out = pipeline.CreateEmitter<IStreamingSpeechRecognitionResult>(this, nameof(this.stt2Out));
            
            positionrotation1Writer = new StreamWriter($@"{path}_1_position_rotation.csv");
            positionrotation2Writer = new StreamWriter($@"{path}_2_position_rotation.csv");
            gazeEventWriter = new StreamWriter($@"{path}_users_gaze_event.csv");
            taskInteractionEventWriter = new StreamWriter($@"{path}_users_task_interaction_event.csv");
            streamWriterList.Add(positionrotation1Writer);
            streamWriterList.Add(positionrotation2Writer);
            streamWriterList.Add(gazeEventWriter);
            streamWriterList.Add(taskInteractionEventWriter);
            /*gazeFilteredEventWriter = new StreamWriter($@"{path}_users_gaze_filtered_event.csv");
            IndexesWriter = new StreamWriter($@"{path}_individual_and_team_metrics_indexes.csv");
            profileConfidenceWriter = new StreamWriter($@"{path}_profiles_confidence_indexes.csv");*/
            //streamWriterList.Add(gazeFilteredEventWriter);
            //streamWriterList.Add(IndexesWriter);
            //streamWriterList.Add(profileConfidenceWriter);

            if (!isHeadup)
            {
                positionrotation1Writer.WriteLine(headWristHeadupEU);
                positionrotation2Writer.WriteLine(headWristHeadupEU);
                gazeEventWriter.WriteLine(gazeEventHeadupEU);
              //gazeFilteredEventWriter.WriteLine(gazeEventFilteredHeadupEU);
                taskInteractionEventWriter.WriteLine(taskInteractionHeadupEU);

                isHeadup = true;
            }
        }

        private void ReceiveTimer(TimeSpan span, Envelope envelope)
        {
            // Add process to the data
        }

        private void ReceiveVideo1(Shared<EncodedImage> shared, Envelope envelope)
        {
            // Add process to the data
            Video1Out.Post(shared, envelope.OriginatingTime);
        }

        private void ReceiveVideo2(Shared<EncodedImage> shared, Envelope envelope)
        {
            // Add process to the data
            Video2Out.Post(shared, envelope.OriginatingTime);
        }

        private void ReceiveBodiesKinect(List<AzureKinectBody> list, Envelope envelope)
        {
            // Add process to the data
            bodiesOut.Post(list, envelope.OriginatingTime);
        }

        private void ReceiveHead1(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            string positionFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;{tuple.Item1.X};{tuple.Item1.Y};{tuple.Item1.Z};{tuple.Item2.X};{tuple.Item2.Y};{tuple.Item2.Z};{currentLeftHand1.Item1.X};{currentLeftHand1.Item1.Y};{currentLeftHand1.Item1.Z};{currentLeftHand1.Item2.X};{currentLeftHand1.Item2.Y};{currentLeftHand1.Item2.Z};{currentRightHand1.Item1.X};{currentRightHand1.Item1.Y};{currentRightHand1.Item1.Z};{currentRightHand1.Item2.X};{currentRightHand1.Item2.Y};{currentRightHand1.Item2.Z}".Replace(',', '.');
            positionrotation1Writer.WriteLine(positionFormat);
            
            Head1Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveHead2(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            string positionFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;{tuple.Item1.X};{tuple.Item1.Y};{tuple.Item1.Z};{tuple.Item2.X};{tuple.Item2.Y};{tuple.Item2.Z};{currentLeftHand2.Item1.X};{currentLeftHand2.Item1.Y};{currentLeftHand2.Item1.Z};{currentLeftHand2.Item2.X};{currentLeftHand2.Item2.Y};{currentLeftHand2.Item2.Z};{currentRightHand2.Item1.X};{currentRightHand2.Item1.Y};{currentRightHand2.Item1.Z};{currentRightHand2.Item2.X};{currentRightHand2.Item2.Y};{currentRightHand2.Item2.Z}".Replace(',', '.');
            positionrotation2Writer.WriteLine(positionFormat);
            
            Head2Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveLeftHand1(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            currentLeftHand1 = tuple;

            LeftHand1Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveLeftHand2(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            currentLeftHand2 = tuple;

            LeftHand2Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveRightHand1(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            currentRightHand1 = tuple;

            RightHand1Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveRightHand2(Tuple<Vector3, Vector3> tuple, Envelope envelope)
        {
            // Add process to the data
            currentRightHand2 = tuple;

            RightHand2Out.Post(tuple, envelope.OriginatingTime);
        }

        private void ReceiveOtherEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            if(data.type.Equals("step"))
            {
                if (data.step == 3 && startTask == DateTime.MaxValue)
                {
                    startTask = data.originatingTime;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Start Task");
                    Console.ResetColor();
                }
                else if (data.step == 5 && endTask == DateTime.MinValue)
                {
                    endTask = data.originatingTime;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("EndTask \nTask completion time: " + (endTask - startTask).TotalSeconds);
                    Console.ResetColor();
                }
            }
            
            OtherEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveGazeEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;object;IN;1;{data.objectID}".Replace(',', '.'); //IN
            gazeEventWriter.WriteLine(gazeEventFormat);

            GazeEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUngazeEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;object;OUT;1;{data.objectID}".Replace(',', '.'); //OUT
            gazeEventWriter.WriteLine(gazeEventFormat);

            UngazeEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveHoverEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            HoverEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUnhoverEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            UnhoverEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveSelectEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.blockState.Contains("POSITIONNED"))
            {
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;CHANGE COLOR;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }
            else if (data.blockState.Contains("TARGET"))
            {
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;TRY TO POSITION;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }

            SelectEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUnselectEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            UnselectEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveActivateEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.blockState.Contains("TAKEN") && !positionnedPiece[ObjectIDNumber(data.objectID)].taken && data.objectID.Contains("Room"))
            {
                positionnedPiece[ObjectIDNumber(data.objectID)].originatingTimeTaken = data.originatingTime;
                positionnedPiece[ObjectIDNumber(data.objectID)].taken = true;
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;TAKE;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }
            else if (data.blockState.Contains("POSITIONNED") && !positionnedPiece[ObjectIDNumber(data.objectID)].positionned && data.objectID.Contains("Place"))
            {
                positionnedPiece[ObjectIDNumber(data.objectID)].originatingTimePositionned = data.originatingTime;
                positionnedPiece[ObjectIDNumber(data.objectID)].positionned = true;
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};1;POSITIONNED;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }

            ActivateEvent1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveOtherEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            OtherEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveGazeEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;object;IN;2;{data.objectID}".Replace(',', '.'); //IN
            gazeEventWriter.WriteLine(gazeEventFormat);

            GazeEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUngazeEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;object;OUT;2;{data.objectID}".Replace(',', '.'); //IN
            gazeEventWriter.WriteLine(gazeEventFormat);

            UngazeEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveHoverEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            HoverEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUnhoverEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            UnhoverEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveSelectEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.blockState.Contains("POSITIONNED"))
            {
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;CHANGE COLOR;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }
            else if (data.blockState.Contains("TARGET"))
            {
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;TRY TO POSITION;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }

            SelectEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveUnselectEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            UnselectEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveActivateEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.blockState.Contains("TAKEN") && !positionnedPiece[ObjectIDNumber(data.objectID)].taken && data.objectID.Contains("Room"))
            {
                positionnedPiece[ObjectIDNumber(data.objectID)].originatingTimeTaken = data.originatingTime;
                positionnedPiece[ObjectIDNumber(data.objectID)].taken = true;
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;TAKE;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);

            }
            else if (data.blockState.Contains("POSITIONNED") && !positionnedPiece[ObjectIDNumber(data.objectID)].positionned && data.objectID.Contains("Place"))
            {
                positionnedPiece[ObjectIDNumber(data.objectID)].originatingTimePositionned = data.originatingTime;
                positionnedPiece[ObjectIDNumber(data.objectID)].positionned = true;
                string message = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};2;POSITIONNED;{data.objectID}";
                taskInteractionEventWriter.WriteLine(message);
            }
            ActivateEvent2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveGazeAvatarEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.gaze)
            {
                string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};{data.userIDGazer};user;IN;{data.userIDGazer};{data.userIDGazed}".Replace(',', '.'); //IN
                gazeEventWriter.WriteLine(gazeEventFormat);
            }
            else if (!data.gaze)
            {
                string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};{data.userIDGazer};user;OUT;{data.userIDGazer};{data.userIDGazed}".Replace(',', '.'); //OUT
                gazeEventWriter.WriteLine(gazeEventFormat);
            }
            GazeAvatar1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveValidationPuzzleEvent1(EventData data, Envelope envelope)
        {
            // Add process to the data
            ValidationPuzzle1Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveGazeAvatarEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            if (data.gaze)
            {
                string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};{data.userIDGazer};user;IN;{data.userIDGazer};{data.userIDGazed}".Replace(',', '.'); //IN
                gazeEventWriter.WriteLine(gazeEventFormat);
            }
            else if (!data.gaze)
            {
                string gazeEventFormat = $"{envelope.OriginatingTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString().Replace(',', '.')};{data.userIDGazer};user;OUT;{data.userIDGazer};{data.userIDGazed}".Replace(',', '.'); //OUT
                gazeEventWriter.WriteLine(gazeEventFormat);
            }
            GazeAvatar2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveValidationPuzzleEvent2(EventData data, Envelope envelope)
        {
            // Add process to the data
            ValidationPuzzle2Out.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveCollabEvent(CollabEventData data, Envelope envelope)
        {
            // Add process to the data
            CollabEventOut.Post(data, envelope.OriginatingTime);
        }

        private void ReceiveVad1Result(bool arg1, Envelope envelope)
        {
            // Add process to the data
            vad1Out.Post(arg1, envelope.OriginatingTime);
        }

        private void ReceiveVad2Result(bool arg1, Envelope envelope)
        {
            // Add process to the data
            vad2Out.Post(arg1, envelope.OriginatingTime);
        }

        private void ReceiveStt1Result(IStreamingSpeechRecognitionResult result, Envelope envelope)
        {
            // Add process to the data
            stt1Out.Post(result, envelope.OriginatingTime);
        }

        private void ReceiveStt2Result(IStreamingSpeechRecognitionResult result, Envelope envelope)
        {
            // Add process to the data
            stt2Out.Post(result, envelope.OriginatingTime);
        }
        public void Initialize()
        {
            
        }
        public int ObjectIDNumber(string id)
        {
            int value = 0;
            switch (id)
            {
                case string s when id.Contains("TBloc"):
                    value = 0;
                    break;
                case string s when id.Contains("LBloc"):
                    value = 1;
                    break;
                case string s when id.Contains("SBloc"):
                    value = 2;
                    break;
                case string s when id.Contains("ZBloc"):
                    value = 3;
                    break;
                case string s when id.Contains("LineBloc"):
                    value = 4;
                    break;
                case string s when id.Contains("CubeBloc"):
                    value = 5;
                    break;
                case string s when id.Contains("JBloc"):
                    value = 6;
                    break;
            }
            return value;
        }

        public Receiver<Tuple<Vector3, Vector3>> Head1In { get; private set; }
        public Receiver<Tuple<Vector3, Vector3>> Head2In { get; private set; }
        public Receiver<Tuple<Vector3, Vector3>> LeftHand1In { get; private set; }
        public Receiver<Tuple<Vector3, Vector3>> LeftHand2In { get; private set; }
        public Receiver<Tuple<Vector3, Vector3>> RightHand1In { get; private set; }
        public Receiver<Tuple<Vector3, Vector3>> RightHand2In { get; private set; }
        public Receiver<Shared<EncodedImage>> Video1In { get; private set; }
        public Receiver<Shared<EncodedImage>> Video2In { get; private set; }

        public Receiver<EventData> OtherEvent1In { get; private set; }
        public Receiver<EventData> GazeEvent1In { get; private set; }
        public Receiver<EventData> UngazeEvent1In { get; private set; }
        public Receiver<EventData> HoverEvent1In { get; private set; }
        public Receiver<EventData> UnhoverEvent1In { get; private set; }
        public Receiver<EventData> SelectEvent1In { get; private set; }
        public Receiver<EventData> UnselectEvent1In { get; private set; }
        public Receiver<EventData> ActivateEvent1In { get; private set; }
        public Receiver<EventData> UnactivateEvent1In { get; private set; }
        
        public Receiver<EventData> OtherEvent2In { get; private set; }
        public Receiver<EventData> GazeEvent2In { get; private set; }
        public Receiver<EventData> UngazeEvent2In { get; private set; }
        public Receiver<EventData> HoverEvent2In { get; private set; }
        public Receiver<EventData> UnhoverEvent2In { get; private set; }
        public Receiver<EventData> SelectEvent2In { get; private set; }
        public Receiver<EventData> UnselectEvent2In { get; private set; }
        public Receiver<EventData> ActivateEvent2In { get; private set; }
        public Receiver<EventData> UnactivateEvent2In { get; private set; }

        public Receiver<EventData> GazeAvatar1In { get; private set; }
        public Receiver<EventData> ValidationPuzzle1In { get; private set; }
        public Receiver<EventData> GazeAvatar2In { get; private set; }
        public Receiver<EventData> ValidationPuzzle2In { get; private set; }
        public Receiver<CollabEventData> CollabEventIn { get; private set; }
        public Receiver<List<AzureKinectBody>> bodiesIn { get; private set; }

        public Receiver<bool> vad1In { get; private set; }
        public Receiver<bool> vad2In { get; private set; }
        public Receiver<IStreamingSpeechRecognitionResult> stt1In { get; private set; }
        public Receiver<IStreamingSpeechRecognitionResult> stt2In { get; private set; }

        public Receiver< TimeSpan> timerIn { get; private set; }

        // Raw data emitters
        public Emitter<Shared<EncodedImage>> Video1Out { get; private set; }
        public Emitter<Shared<EncodedImage>> Video2Out { get; private set; }
        public Emitter<List<AzureKinectBody>> bodiesOut { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> Head1Out { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> Head2Out { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> LeftHand1Out { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> LeftHand2Out { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> RightHand1Out { get; private set; }
        public Emitter<Tuple<Vector3, Vector3>> RightHand2Out { get; private set; }
        public Emitter<EventData> OtherEvent1Out { get; private set; }
        public Emitter<EventData> GazeEvent1Out { get; private set; }
        public Emitter<EventData> UngazeEvent1Out { get; private set; }
        public Emitter<EventData> HoverEvent1Out { get; private set; }
        public Emitter<EventData> UnhoverEvent1Out { get; private set; }
        public Emitter<EventData> SelectEvent1Out { get; private set; }
        public Emitter<EventData> UnselectEvent1Out { get; private set; }
        public Emitter<EventData> ActivateEvent1Out { get; private set; }
        public Emitter<EventData> OtherEvent2Out { get; private set; }
        public Emitter<EventData> GazeEvent2Out { get; private set; }
        public Emitter<EventData> UngazeEvent2Out { get; private set; }
        public Emitter<EventData> HoverEvent2Out { get; private set; }
        public Emitter<EventData> UnhoverEvent2Out { get; private set; }
        public Emitter<EventData> SelectEvent2Out { get; private set; }
        public Emitter<EventData> UnselectEvent2Out { get; private set; }
        public Emitter<EventData> ActivateEvent2Out { get; private set; }
        public Emitter<bool> vad1Out { get; private set; }
        public Emitter<bool> vad2Out { get; private set; }
        public Emitter<IStreamingSpeechRecognitionResult> stt1Out { get; private set; }
        public Emitter<IStreamingSpeechRecognitionResult> stt2Out { get; private set; }
        public Emitter<EventData> GazeAvatar1Out { get; private set; }
        public Emitter<EventData> GazeAvatar2Out { get; private set; }
        public Emitter<EventData> ValidationPuzzle1Out { get; private set; }
        public Emitter<EventData> ValidationPuzzle2Out { get; private set; }

        public Emitter<CollabEventData> CollabEventOut { get; private set; }
    }
}
