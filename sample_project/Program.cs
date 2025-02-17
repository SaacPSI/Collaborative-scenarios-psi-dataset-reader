using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Psi;
using Microsoft.Psi.Data;
using Microsoft.Psi.Speech;
using Microsoft.Psi.Imaging;
using Microsoft.Psi.AzureKinect;
using sample_project.specific_class;
using sample_project;

namespace CSCW_dataset_sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var listargs = new List<string>
            {
                @"...\CSCW25_collaboration_profiles_dataset\", // Existing dataset file path
                "...", // Collaborative Scenario Name
                "C", // Disk Letter
                @":\...\" // New dataset file path
            };

            using (var p = Pipeline.Create(enableDiagnostics: true))
            {
                Dataset dataset;
                PsiExporter videoStore, bodiesKinectStore, positionStore, eventStore1, eventStore2, vadStore, sttStore, collabenveventStore, eventStorePrint;
                SetupCreateNewDataset(listargs, p, out dataset, out videoStore, out bodiesKinectStore, out positionStore, out eventStore1, out eventStore2, out vadStore, out sttStore, out collabenveventStore, out eventStorePrint);

                // Save dataset
                dataset.SaveAs($@"{listargs[2]}{listargs[3]}{listargs[1]}\{listargs[1]}.pds");

                // Read stores for the opening session
                var videoStoreRaw = new PsiStoreStreamReader("Recorded-Video", $"{listargs[0]}{listargs[1]}");
                var bodiesKinectStoreRaw = new PsiStoreStreamReader("Recorded-Skeletons", $"{listargs[0]}{listargs[1]}");
                var positionStoreRaw = new PsiStoreStreamReader("Recorded-Position_Rotation", $"{listargs[0]}{listargs[1]}");
                var eventStoreRaw1 = new PsiStoreStreamReader("Recorded-Interaction1", $"{listargs[0]}{listargs[1]}");
                var eventStoreRaw2 = new PsiStoreStreamReader("Recorded-Interaction2", $"{listargs[0]}{listargs[1]}");
                var vadStoreRaw = new PsiStoreStreamReader("Processed-VAD", $"{listargs[0]}{listargs[1]}");
                var sttStoreRaw = new PsiStoreStreamReader("Processed-STT", $"{listargs[0]}{listargs[1]}");
                var collabenveventStoreRawn = new PsiStoreStreamReader("Recorded-CollaborativeEvent", $"{listargs[0]}{listargs[1]}");
                var eventStorePrintRaw = new PsiStoreStreamReader("Print-Events", $"{listargs[0]}{listargs[1]}");
                var processDatastoreRaw = new PsiStoreStreamReader("Processed-IndividualMetrics", $"{listargs[0]}{listargs[1]}");
                var metricDatastoreRaw = new PsiStoreStreamReader("Processed-TeamMetrics", $"{listargs[0]}{listargs[1]}");

                // Open stores for the opening session
                var videoStoreRawOpen = PsiStore.Open(p, "Recorded-Video", $"{listargs[0]}{listargs[1]}");
                var bodiesKinectStoreRawOpen = PsiStore.Open(p, "Recorded-Skeletons", $"{listargs[0]}{listargs[1]}");
                var positionStoreRawOpen = PsiStore.Open(p, "Recorded-Position_Rotation", $"{listargs[0]}{listargs[1]}");
                var eventStoreRaw1Open = PsiStore.Open(p, "Recorded-Interaction1", $"{listargs[0]}{listargs[1]}");
                var eventStoreRaw2Open = PsiStore.Open(p, "Recorded-Interaction2", $"{listargs[0]}{listargs[1]}");
                var vadStoreRawOpen = PsiStore.Open(p, "Processed-VAD", $"{listargs[0]}{listargs[1]}");
                var sttStoreRawOpen = PsiStore.Open(p, "Processed-STT", $"{listargs[0]}{listargs[1]}");
                var collabenveventStoreRawOpen = PsiStore.Open(p, "Recorded-CollaborativeEvent", $"{listargs[0]}{listargs[1]}");
                var eventStorePrintOpen = PsiStore.Open(p, "Print-Events", $"{listargs[0]}{listargs[1]}");
                var processDatastoreOpen = PsiStore.Open(p, "Processed-IndividualMetrics", $"{listargs[0]}{listargs[1]}");
                var metricDatastoreOpen = PsiStore.Open(p, "Processed-TeamMetrics", $"{listargs[0]}{listargs[1]}");

                // Open specific streams recorded on stores
                // VR View
                var video1 = videoStoreRawOpen.OpenStream<Shared<EncodedImage>>("1_View");
                var video2 = videoStoreRawOpen.OpenStream<Shared<EncodedImage>>("2_View");
                // Skeletons
                var bodies = bodiesKinectStoreRawOpen.OpenStream<List<AzureKinectBody>>("KinectBodies");
                // Users Head, Left and Right hand position & rotation
                var head1 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("1_Head");
                var head2 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("2_Head");
                var left1 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("1_LeftHand");
                var left2 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("2_LeftHand");
                var right1 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("1_RightHand");
                var right2 = positionStoreRawOpen.OpenStream<Tuple<Vector3, Vector3>>("2_RightHand");
                // Users Interactions
                var otherevent1 = eventStoreRaw1Open.OpenStream<EventData>("1_Other");
                var gaze1 = eventStoreRaw1Open.OpenStream<EventData>("1_Gaze");
                var ungaze1 = eventStoreRaw1Open.OpenStream<EventData>("1_Ungaze");
                var hover1 = eventStoreRaw1Open.OpenStream<EventData>("1_Hover");
                var unhover1 = eventStoreRaw1Open.OpenStream<EventData>("1_Unhover");
                var select1 = eventStoreRaw1Open.OpenStream<EventData>("1_Select");
                var unselect1 = eventStoreRaw1Open.OpenStream<EventData>("1_Unselect");
                var activate1 = eventStoreRaw1Open.OpenStream<EventData>("1_Activate");
                var unactivate1 = eventStoreRaw1Open.OpenStream<EventData>("1_Unactivate");
                var otherevent2 = eventStoreRaw2Open.OpenStream<EventData>("2_Other");
                var gaze2 = eventStoreRaw2Open.OpenStream<EventData>("2_Gaze");
                var ungaze2 = eventStoreRaw2Open.OpenStream<EventData>("2_Ungaze");
                var hover2 = eventStoreRaw2Open.OpenStream<EventData>("2_Hover");
                var unhover2 = eventStoreRaw2Open.OpenStream<EventData>("2_Unhover");
                var select2 = eventStoreRaw2Open.OpenStream<EventData>("2_Select");
                var unselect2 = eventStoreRaw2Open.OpenStream<EventData>("2_Unselect");
                var activate2 = eventStoreRaw2Open.OpenStream<EventData>("2_Activate");
                var unactivate2 = eventStoreRaw2Open.OpenStream<EventData>("2_Unactivate");
                // Users Voice activity detection boolean
                var vad1 = vadStoreRawOpen.OpenStream<bool>("1_VAD");
                var vad2 = vadStoreRawOpen.OpenStream<bool>("2_VAD");
                // Users speech-to-text
                var stt1 = sttStoreRawOpen.OpenStream<IStreamingSpeechRecognitionResult>("1_STT");
                var stt2 = sttStoreRawOpen.OpenStream<IStreamingSpeechRecognitionResult>("2_STT");

                var avatargaze1 = collabenveventStoreRawOpen.OpenStream<EventData>("1_Avatar Gaze");
                var avatargaze2 = collabenveventStoreRawOpen.OpenStream<EventData>("2_Avatar Gaze");
                var validationPuzzle1 = collabenveventStoreRawOpen.OpenStream<EventData>("1_ValidationPuzzle");//ValidationPuzzle1Raw / P1validationPuzzleRaw
                var validationPuzzle2 = collabenveventStoreRawOpen.OpenStream<EventData>("1_ValidationPuzzle");//ValidationPuzzle2Raw / P2validationPuzzleRaw
                var collabeventraw = collabenveventStoreRawOpen.OpenStream<CollabEventData>("Processed-CollaborativeEvent");

                string path = $@"{listargs[2]}{listargs[3]}{listargs[1]}\{listargs[1]}";
                Receiver receiver = new Receiver(p, path);

                // Pipe stream from the dataset to their receivers
                PipeExistingStreamToReceivers(video1, video2, bodies, head1, head2, left1, left2, right1, right2, otherevent1, gaze1, ungaze1, hover1, unhover1, select1, unselect1, activate1, otherevent2, gaze2, ungaze2, hover2, unhover2, select2, unselect2, activate2, vad1, vad2, stt1, stt2, avatargaze1, avatargaze2, validationPuzzle1, validationPuzzle2, collabeventraw, receiver);

                var timer = Timers.Timer(p, TimeSpan.FromMilliseconds(33));
                timer.PipeTo(receiver.timerIn);

                // Set and write print stream based on processed streams
                SetAndStorePrintStreams(eventStorePrint, receiver);

                // Write to store the recorded stream
                WriteDataStreamInNewDataset(videoStore, bodiesKinectStore, positionStore, eventStore1, eventStore2, vadStore, sttStore, collabenveventStore, receiver);

                dataset.Save();
                p.Run(ReplayDescriptor.ReplayAllRealTime);

                Console.WriteLine("Press any key to stop the application...");
                Console.ReadLine();
                Console.WriteLine("Closing the application...");
                p.Dispose();
                foreach (var value in receiver.streamWriterList)
                {
                    value.Flush();
                    value.Close();
                    value.Dispose();
                }
            }
        }

        private static void SetAndStorePrintStreams(PsiExporter eventStorePrint, Receiver receiver)
        {
            var other1Print = receiver.OtherEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var gaze1Print = receiver.GazeEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var ungaze1Print = receiver.UngazeEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var hover1Print = receiver.HoverEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var unhover1Print = receiver.UnhoverEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var select1Print = receiver.SelectEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var unselect1Print = receiver.UnselectEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var activate1Print = receiver.ActivateEvent1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);

            var other2Print = receiver.OtherEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var gaze2Print = receiver.GazeEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var ungaze2Print = receiver.UngazeEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var hover2Print = receiver.HoverEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var unhover2Print = receiver.UnhoverEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var select2Print = receiver.SelectEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var unselect2Print = receiver.UnselectEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var activate2Print = receiver.ActivateEvent2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);

            var gazeAvatar1Print = receiver.GazeAvatar1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var gazeAvatar2Print = receiver.GazeAvatar2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var validationPuzzle1Print = receiver.ValidationPuzzle1Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);
            var validationPuzzle2Print = receiver.ValidationPuzzle2Out.Select(d => d.type + ";" + d.time + ";" + d.buttonValidation + ";" + d.gaze + ";" + d.actionOnPieces + ";" + d.userID + ";" + d.objectID + ";" + d.blockState + ";" + d.step + ";" + d.userIDGazed + ";" + d.userIDGazer + ";" + d.interruptionNum + ";" + d.interruptionSuccess);

            var collabEventPrint = receiver.CollabEventOut.Select(d => d.firstUser + ";" + d.secondUser + ";" + d.collabAction + ";" + d.objectIfexist + ";" + d.isBegin + ";" + d.requireSpecificCue + ";" + d.time);
            var sttsegmented1 = receiver.stt1Out.Select(d => d.Text + "_;_" + d.Duration);
            var sttsegmented2 = receiver.stt2Out.Select(d => d.Text + "_;_" + d.Duration);

            eventStorePrint.Write(other1Print, "1_Other_Print");
            eventStorePrint.Write(gaze1Print, "1_Gaze_Print");
            eventStorePrint.Write(ungaze1Print, "1_Ungaze_Print");
            eventStorePrint.Write(hover1Print, "1_Hover_Print");
            eventStorePrint.Write(unhover1Print, "1_Unhover_Print");
            eventStorePrint.Write(select1Print, "1_Select_Print");
            eventStorePrint.Write(unselect1Print, "1_Unselect_Print");
            eventStorePrint.Write(activate1Print, "1_Activate_Print");
            eventStorePrint.Write(other2Print, "2_Other_Print");
            eventStorePrint.Write(gaze2Print, "2_Gaze_Print");
            eventStorePrint.Write(ungaze2Print, "2_Ungaze_Print");
            eventStorePrint.Write(hover2Print, "2_Hover_Print");
            eventStorePrint.Write(unhover2Print, "2_Unhover_Print");
            eventStorePrint.Write(select2Print, "2_Select_Print");
            eventStorePrint.Write(unselect2Print, "2_Unselect_Print");
            eventStorePrint.Write(activate2Print, "2_Activate_Print");
            eventStorePrint.Write(gazeAvatar1Print, "1_AvatarGaze_Print");
            eventStorePrint.Write(gazeAvatar2Print, "2_AvatarGaze_Print");
            eventStorePrint.Write(validationPuzzle1Print, "1_ValidationPuzzle_Print");
            eventStorePrint.Write(validationPuzzle2Print, "2_ValidationPuzzle_Print");
            eventStorePrint.Write(collabEventPrint, "Collaborative Event_Print");
        }
        private static void SetupCreateNewDataset(List<string> listargs, Pipeline p, out Dataset dataset, out PsiExporter videoStore, out PsiExporter bodiesKinectStore, out PsiExporter positionStore, out PsiExporter eventStore1, out PsiExporter eventStore2, out PsiExporter vadStore, out PsiExporter sttStore, out PsiExporter collabenveventStore, out PsiExporter eventStorePrint)
        {
            // Create new dataset
            dataset = new Dataset("DataCollection");
            // Create session for the current dataset
            var session = dataset.CreateSession(listargs[1]);

            // Create psi store
            videoStore = PsiStore.Create(p, "Recorded-Video", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            bodiesKinectStore = PsiStore.Create(p, "Recorded-Skeletons", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            positionStore = PsiStore.Create(p, "Recorded-Position_Rotation", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            eventStore1 = PsiStore.Create(p, "Recorded-Interaction1", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            eventStore2 = PsiStore.Create(p, "Recorded-Interaction2", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            vadStore = PsiStore.Create(p, "Processed-VAD", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            sttStore = PsiStore.Create(p, "Processed-STT", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            collabenveventStore = PsiStore.Create(p, "Recorded-CollaborativeEvent", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            eventStorePrint = PsiStore.Create(p, "Print-Events", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            //var individualmetricDatastore = PsiStore.Create(p, "Processed-IndividualMetrics", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            //var metricDatastore = PsiStore.Create(p, "Processed-TeamMetrics", $@"{listargs[2]}{listargs[3]}{listargs[1]}");
            //var profilConfidenceDatastore = PsiStore.Create(p, "Processed-ProfileConfidence", $@"{listargs[2]}{listargs[3]}{listargs[1]}");

            // Create partitions and add it to the current session
            var videoRPartition = session.AddPsiStorePartition("Recorded-Video", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Video");
            var bodiesKinectRPartition = session.AddPsiStorePartition("Recorded-Skeletons", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Skeletons");
            var posRPartition = session.AddPsiStorePartition("Recorded-Position_Rotation", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Position_Rotation");
            var event1RPartition = session.AddPsiStorePartition("Recorded-Interaction1", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Interaction User 1");
            var event2RPartition = session.AddPsiStorePartition("Recorded-Interaction2", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Interaction User 2");
            var vadRPartition = session.AddPsiStorePartition("Processed-VAD", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Processed-Voice Activity Detector");
            var sttRPartition = session.AddPsiStorePartition("Processed-STT", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Processed-Speech To Text");
            var collabenveventRPartition = session.AddPsiStorePartition("Recorded-CollaborativeEvent", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Recorded-Collaborative Event");
            var eventPPartition = session.AddPsiStorePartition("Print-Events", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Print-Events");
            //var individualmetricsPartition = session.AddPsiStorePartition("Processed-IndividualMetrics", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Processed-Individual Metrics");
            //var metricsPartition = session.AddPsiStorePartition("Processed-TeamMetrics", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Processed-Team Metrics");
            //var profilesPartition = session.AddPsiStorePartition("Processed-ProfileConfidence", $@"{listargs[2]}{listargs[3]}{listargs[1]}", "Processed-Profile Confidence");
        }
        private static void PipeExistingStreamToReceivers(IProducer<Shared<EncodedImage>> video1, IProducer<Shared<EncodedImage>> video2, IProducer<List<AzureKinectBody>> bodies, IProducer<Tuple<Vector3, Vector3>> head1, IProducer<Tuple<Vector3, Vector3>> head2, IProducer<Tuple<Vector3, Vector3>> left1, IProducer<Tuple<Vector3, Vector3>> left2, IProducer<Tuple<Vector3, Vector3>> right1, IProducer<Tuple<Vector3, Vector3>> right2, IProducer<EventData> otherevent1, IProducer<EventData> gaze1, IProducer<EventData> ungaze1, IProducer<EventData> hover1, IProducer<EventData> unhover1, IProducer<EventData> select1, IProducer<EventData> unselect1, IProducer<EventData> activate1, IProducer<EventData> otherevent2, IProducer<EventData> gaze2, IProducer<EventData> ungaze2, IProducer<EventData> hover2, IProducer<EventData> unhover2, IProducer<EventData> select2, IProducer<EventData> unselect2, IProducer<EventData> activate2, IProducer<bool> vad1, IProducer<bool> vad2, IProducer<IStreamingSpeechRecognitionResult> stt1, IProducer<IStreamingSpeechRecognitionResult> stt2, IProducer<EventData> avatargaze1, IProducer<EventData> avatargaze2, IProducer<EventData> validationPuzzle1, IProducer<EventData> validationPuzzle2, IProducer<CollabEventData> collabeventraw, Receiver receiver)
        {
            // Pipe open stream to receiver
            video1.PipeTo(receiver.Video1In);
            video2.PipeTo(receiver.Video2In);

            bodies.PipeTo(receiver.bodiesIn);

            head1.PipeTo(receiver.Head1In);
            head2.PipeTo(receiver.Head2In);
            left1.PipeTo(receiver.LeftHand1In);
            left2.PipeTo(receiver.LeftHand2In);
            right1.PipeTo(receiver.RightHand1In);
            right2.PipeTo(receiver.RightHand2In);

            otherevent1.PipeTo(receiver.OtherEvent1In);
            gaze1.PipeTo(receiver.GazeEvent1In);
            ungaze1.PipeTo(receiver.UngazeEvent1In);
            hover1.PipeTo(receiver.HoverEvent1In);
            unhover1.PipeTo(receiver.UnhoverEvent1In);
            select1.PipeTo(receiver.SelectEvent1In);
            unselect1.PipeTo(receiver.UnselectEvent1In);
            activate1.PipeTo(receiver.ActivateEvent1In);

            otherevent2.PipeTo(receiver.OtherEvent2In);
            gaze2.PipeTo(receiver.GazeEvent2In);
            ungaze2.PipeTo(receiver.UngazeEvent2In);
            hover2.PipeTo(receiver.HoverEvent2In);
            unhover2.PipeTo(receiver.UnhoverEvent2In);
            select2.PipeTo(receiver.SelectEvent2In);
            unselect2.PipeTo(receiver.UnselectEvent2In);
            activate2.PipeTo(receiver.ActivateEvent2In);

            vad1.PipeTo(receiver.vad1In);
            vad2.PipeTo(receiver.vad2In);

            stt1.PipeTo(receiver.stt1In);
            stt2.PipeTo(receiver.stt2In);

            avatargaze1.PipeTo(receiver.GazeAvatar1In);
            avatargaze2.PipeTo(receiver.GazeAvatar2In);
            validationPuzzle1.PipeTo(receiver.ValidationPuzzle1In);
            validationPuzzle2.PipeTo(receiver.ValidationPuzzle2In);
            collabeventraw.PipeTo(receiver.CollabEventIn);
        }
        private static void WriteDataStreamInNewDataset(PsiExporter videoStore, PsiExporter bodiesKinectStore, PsiExporter positionStore, PsiExporter eventStore1, PsiExporter eventStore2, PsiExporter vadStore, PsiExporter sttStore, PsiExporter collabenveventStore, Receiver receiver)
        {
            videoStore.Write(receiver.Video1Out, "1_View");
            videoStore.Write(receiver.Video2Out, "2_View");
            bodiesKinectStore.Write(receiver.bodiesOut, "KinectBodies");

            positionStore.Write(receiver.Head1Out, "1_Head");
            positionStore.Write(receiver.Head2Out, "2_Head");
            positionStore.Write(receiver.LeftHand1Out, "1_LeftHand");
            positionStore.Write(receiver.LeftHand2Out, "2_LeftHand");
            positionStore.Write(receiver.RightHand1Out, "1_RightHand");
            positionStore.Write(receiver.RightHand2Out, "2_RightHand");

            eventStore1.Write(receiver.OtherEvent1Out, "1_Other");
            eventStore1.Write(receiver.GazeAvatar1Out, "1_Gaze");
            eventStore1.Write(receiver.UngazeEvent1Out, "1_Ungaze");
            eventStore1.Write(receiver.HoverEvent1Out, "1_Hover");
            eventStore1.Write(receiver.UnhoverEvent1Out, "1_Unhover");
            eventStore1.Write(receiver.SelectEvent1Out, "1_Select");
            eventStore1.Write(receiver.UnselectEvent1Out, "1_Unselect");
            eventStore1.Write(receiver.ActivateEvent1Out, "1_Activate");

            eventStore2.Write(receiver.OtherEvent2Out, "2_Other");
            eventStore2.Write(receiver.GazeEvent2Out, "2_Gaze");
            eventStore2.Write(receiver.UngazeEvent2Out, "2_Ungaze");
            eventStore2.Write(receiver.HoverEvent2Out, "2_Hover");
            eventStore2.Write(receiver.UnhoverEvent2Out, "2_Unhover");
            eventStore2.Write(receiver.SelectEvent2Out, "2_Select");
            eventStore2.Write(receiver.UnselectEvent2Out, "2_Unselect");
            eventStore2.Write(receiver.ActivateEvent2Out, "2_Activate");

            vadStore.Write(receiver.vad1Out, "1_VAD");
            vadStore.Write(receiver.vad2Out, "2_VAD");
            sttStore.Write(receiver.stt1Out, "1_STT");
            sttStore.Write(receiver.stt2Out, "2_STT");

            collabenveventStore.Write(receiver.GazeAvatar1Out, "1_Avatar Gaze");
            collabenveventStore.Write(receiver.GazeAvatar2Out, "2_Avatar Gaze");
            collabenveventStore.Write(receiver.ValidationPuzzle1Out, "1_ValidationPuzzle");
            collabenveventStore.Write(receiver.ValidationPuzzle2Out, "2_ValidationPuzzle");
            collabenveventStore.Write(receiver.CollabEventOut, "Processed-CollaborativeEvent");
        }
    }
}
