using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrox.MatroxImagingLibrary;
using HalconDotNet;
using System.Runtime.InteropServices;


namespace MHookFunction_CS
{
    public class MatroxCard
    {
        //private const int MAXBOARDNUM = 5;//板卡的最大数
        private MIL_ID[] MilSystemArray = new MIL_ID[5];

        private static MIL_ID MilApplication = MIL.M_NULL;

        //public List<DigitizerImp> listBaslerImp = new List<DigitizerImp>();
       private int nCurBoardNum = 2;



       private static Dictionary<string, MIL_ID> mapMilSystem = new Dictionary<string, MIL_ID>();

       private static Dictionary<string, MIL_ID> mapMilRealSystem = new Dictionary<string, MIL_ID>();

       private static Dictionary<string, DigitizerImp> mapDigitizerImp = new Dictionary<string, DigitizerImp>();
 
       private static MatroxCard instance;
       private static object _lock=new object();

       private MatroxCard()
       {
           if (MilApplication == MIL.M_NULL)
           {
               MIL.MappAlloc(MIL.M_DEFAULT, ref MilApplication);
           }
       }

       public static MatroxCard GetInstance()
       {
               if(instance==null)
               {
                      lock(_lock)
                      {
                             if(instance==null)
                             {
                                 instance = new MatroxCard();
                             }
                      }
               }
               return instance;
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sBoardType"></param>
        /// <param name="sDcfPath"></param>
        /// <param name="nCamIndex"></param>
        /// <param name="nBoardIndex"></param>
        /// <returns></returns>
        public long Init(string sBoardType,string sDcfPath ,int nCamIndex = 0 ,int nBoardIndex = 0 )
        {
            string strID = nCamIndex.ToString()+","+nBoardIndex.ToString();

            string strRelID =nBoardIndex.ToString();


            MIL_ID MilSystem = MIL.M_NULL;


            if (!mapMilSystem.ContainsKey(strID) )
            {
                if (!mapMilRealSystem.ContainsKey(strRelID))
                {
                    if (sBoardType == "GIGE")
                    {
                        MIL.MsysAlloc(MilApplication, MIL.M_SYSTEM_GIGE_VISION, nBoardIndex, MIL.M_DEFAULT, ref MilSystem);
                    }
                    else if (sBoardType == "SOLIOS")
                    {
                        MIL.MsysAlloc(MilApplication, MIL.M_SYSTEM_SOLIOS, nBoardIndex, MIL.M_DEFAULT, ref MilSystem);

                    }
                    else if (sBoardType == "RADIENTEVCL")
                    {

                        MIL.MsysAlloc(MilApplication, MIL.M_SYSTEM_RADIENTEVCL, nBoardIndex, MIL.M_DEFAULT, ref MilSystem);
                        //MIL.MsysAlloc(MilApplication, sBoardType, nBoardIndex, MIL.M_DEFAULT, ref MilSystem);
                    }
                    else if (sBoardType == "RADIENTCXP")
                    {
                        MIL.MsysAlloc(MilApplication, MIL.M_SYSTEM_RADIENTCXP, nBoardIndex, MIL.M_DEFAULT, ref MilSystem);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(sBoardType + "is not exist !");
                        return -1;
                    }
                    mapMilRealSystem[strRelID] = MilSystem;

                    mapMilSystem[strID] = MilSystem;
                }
                else 
                {
                    mapMilSystem[strID] = mapMilRealSystem[strRelID];          
                
                }    
            }
            else
            { //使用MilSystem
               MilSystem = mapMilSystem[strID] ;            
            }
            
            //分配相机
            if (!mapDigitizerImp.ContainsKey(strID))
            {//分配相机对应
                DigitizerImp pDigitizerImp = new DigitizerImp();
                mapDigitizerImp[strID] = pDigitizerImp;

                pDigitizerImp.Init(nCamIndex, MilSystem, sDcfPath);
            }

            return 0;
        }

        public DigitizerImp GetDigitizerObject(int nCamIndex = 0, int nBoardIndex = 0)
        { 
        
            string strID = nCamIndex.ToString() + "," + nBoardIndex.ToString();
            if (mapDigitizerImp.ContainsKey(strID))
            {
                return mapDigitizerImp[strID];
            }
            return null;
        }

        public void DestroyMatroxCard()
        {
            if (mapDigitizerImp != null)
            {
                foreach (var s in mapDigitizerImp)
                {
                    s.Value.StopGrab();
                    s.Value.Destroy();
                }
            }


            if (mapMilSystem != null)
            {
                foreach (var vsystem in mapMilSystem)
                {
                    MIL_ID tem = vsystem.Value;
                    MIL.MsysFree(tem);
                    tem = MIL.M_NULL;
                }
            }

            if (MilApplication != MIL.M_NULL)
            {
                MIL.MappFree(MilApplication);
                MilApplication = MIL.M_NULL;
            } 
        }

        public long SingleGrab(int nCamIndex = 0, int nBoardIndex = 0)
        {
            string strID = nCamIndex.ToString() + "," + nBoardIndex.ToString();
            if (mapDigitizerImp.ContainsKey(strID))
            {

                mapDigitizerImp[strID].SingleGrab();

            }

            return 0;
        }


        public long Software(int nCamIndex = 0, int nBoardIndex = 0)
        {
            string strID = nCamIndex.ToString() + "," + nBoardIndex.ToString();
            if (mapDigitizerImp.ContainsKey(strID))
            {

                mapDigitizerImp[strID].Software();

            }

            return 0;
        }


        public long GrabContiune(int nCamIndex = 0, int nBoardIndex = 0)
        {
            string strID = nCamIndex.ToString() + "," + nBoardIndex.ToString();
            if (mapDigitizerImp.ContainsKey(strID))
            {

                mapDigitizerImp[strID].ContinuousGrab();

            }

            return 0;
        }


        public long StopGrab(int nCamIndex = 0, int nBoardIndex = 0)
        {
            string strID = nCamIndex.ToString() + "," + nBoardIndex.ToString();
            if (mapDigitizerImp.ContainsKey(strID))
            {

                mapDigitizerImp[strID].StopGrab();

            }

            return 0;
        }

    }


    public class DigitizerImp
    {
        // Number of images in the buffering grab queue.
        // Generally, increasing this number gives a better real-time grab.
        private const int BUFFERING_SIZE_MAX = 2;

        private static MIL_DIG_HOOK_FUNCTION_PTR ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction);

        private GCHandle hUserData;

        public HObject hImage;
        public HWindow hWindow;


        private MIL_ID MilDigitizer = MIL.M_NULL;
        private MIL_ID MilDisplay = MIL.M_NULL;
        private MIL_ID MilImageBuf = MIL.M_NULL;
        private MIL_ID[] MilGrabBufferList = new MIL_ID[BUFFERING_SIZE_MAX];
        int MilGrabBufferListSize = 0;

        private MIL_INT MilImageWidth = MIL.M_NULL;
        private MIL_INT MilImageHeight = MIL.M_NULL;
        private MIL_INT MilImageBand = MIL.M_NULL;

        private GCHandle thObject;
        private byte[] imgData;

        public delegate void delegateProcessHImage(HObject bmp);

        public event delegateProcessHImage eventProcessImage;


        public MIL_INT HookType;
        public MIL_ID HookId;

        public long Init(int nCamIndex, MIL_ID MilSystem, string sDcfPath)
        {

            //HookDataStruct UserHookData = new HookDataStruct();

            if (string.IsNullOrEmpty(sDcfPath))
            {
                MIL.MdigAlloc(MilSystem, nCamIndex, ("M_DEFAULT"), MIL.M_DEFAULT, ref MilDigitizer);
            }
            else
            {
                MIL.MdigAlloc(MilSystem, nCamIndex, sDcfPath, MIL.M_DEFAULT, ref MilDigitizer);
            }

            MIL.MdispAlloc(MilSystem, MIL.M_DEFAULT, ("M_DEFAULT"), MIL.M_DEFAULT, ref MilDisplay);


            // Allocate the grab buffers and clear them.
            MilImageWidth = MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_X, MIL.M_NULL);
            MilImageHeight = MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_Y, MIL.M_NULL);
            MilImageBand = MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_BAND, MIL.M_NULL);

            MIL.MappControl(MIL.M_ERROR, MIL.M_PRINT_DISABLE);

            MIL.MbufAllocColor(MilSystem,
                            MilImageBand,
                            MilImageWidth,
                            MilImageHeight,
                            8 + MIL.M_UNSIGNED,
                            MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC + MIL.M_DISP,     // 参数MIL.M_DISP用于添加显示图片功能
                            ref MilImageBuf);

            MIL.MappControl(MIL.M_ERROR, MIL.M_PRINT_ENABLE);
            MIL.MbufClear(MilImageBuf, 0);
            MIL.MdispSelect(MilDisplay, MilImageBuf);       // 选择用于显示的Buffer
            //MIL.MdispSelectWindow(MilDisplay, MilImageBuf, pictureBox1.Handle);
            //MIL.MdispControl(MilDisplay, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE);

            // Allocate the grab buffers and clear them.
            for (MilGrabBufferListSize = 0; MilGrabBufferListSize < BUFFERING_SIZE_MAX; MilGrabBufferListSize++)
            {
                MIL.MbufAllocColor(MilSystem,
                                MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_BAND, MIL.M_NULL),
                                MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_X, MIL.M_NULL),
                                MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_Y, MIL.M_NULL),
                                8 + MIL.M_UNSIGNED,
                                MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC,
                                ref MilGrabBufferList[MilGrabBufferListSize]);

                if (MilGrabBufferList[MilGrabBufferListSize] != MIL.M_NULL)
                {
                    MIL.MbufClear(MilGrabBufferList[MilGrabBufferListSize], 0xFF);
                }
                else
                {
                    break;
                }
            }


            //********设置相机采集模式和超时时间
            MIL.MdigControl(MilDigitizer, MIL.M_GRAB_MODE, MIL.M_ASYNCHRONOUS);
            MIL.MdigControl(MilDigitizer, MIL.M_GRAB_TIMEOUT, MIL.M_INFINITE);


            imgData = new byte[((int)(MilImageWidth * MilImageHeight * MilImageBand))];

            thObject = GCHandle.Alloc(imgData, GCHandleType.Pinned);

            hUserData = GCHandle.Alloc(this);

            return 0;
        }


        public long ContinuousGrab()
        {
            // Start the processing. The processing function is called with every frame grabbed.
            MIL.MdigProcess(MilDigitizer, MilGrabBufferList, MilGrabBufferListSize, MIL.M_START, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));
            return 0;
        }

        public long StopGrab()
        {
            MIL.MdigProcess(MilDigitizer, MilGrabBufferList, MilGrabBufferListSize, MIL.M_STOP, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

            return 0;
        }

        // 单次采集
        public void SingleGrab()
        {
            MIL.MdigProcess(MilDigitizer, MilGrabBufferList, MilGrabBufferListSize, MIL.M_SEQUENCE + MIL.M_COUNT(1), MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));
        }

        // Timer软触发使能
        public void Software()
        {
            MIL.MdigControl(MilDigitizer, MIL.M_TIMER_TRIGGER_SOFTWARE, MIL.M_ACTIVATE);
        }

        // Local defines.
        private static MIL_INT ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            // get the handle to the DigHookUserData object back from the IntPtr
            GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);


            // get a reference to the DigHookUserData object
            DigitizerImp pThis = hUserData.Target as DigitizerImp;
            pThis.HookId = HookId;
            pThis.HookType = HookType;

            pThis.OnImageGrabbed();

            return 0;
        }

        public virtual void OnImageGrabbed()
        {
            try
            {
                MIL_ID ModifiedBufferId = MIL.M_NULL;

                // 获取图像buffer地址.
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);
                MIL.MbufCopy(ModifiedBufferId, MilImageBuf);


                int bufSizeX = (int)MilImageWidth;
                int bufSizeY = (int)MilImageHeight;
                int bufBand = (int)MilImageBand;


                IntPtr tpObject = thObject.AddrOfPinnedObject();
                MIL.MbufGet(ModifiedBufferId, imgData);


                if (bufBand == 1)
                {
                    HOperatorSet.GenImage1(out hImage, "byte", bufSizeX, bufSizeY, tpObject);
                }
                else if (bufBand == 3)
                {
                    HOperatorSet.GenImageInterleaved(out hImage, tpObject, "bgr",
                          (HTuple)bufSizeX, (HTuple)bufSizeY, -1, "byte", (HTuple)bufSizeX, (HTuple)bufSizeY, 0, 0, -1, 0);
                }



                //UserData.hWindow.SetPart(0, 0, -1, -1);
                //UserData.hImage.DispObj(UserData.hWindow());

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            finally
            {
                eventProcessImage(hImage);
            }
        
        }

        public long Destroy()
        {

            if (thObject.IsAllocated)
            {
                thObject.Free();
            }

            // Free the GCHandle when no longer used
            if (hUserData.IsAllocated)
            {
                hUserData.Free();
            }

            // Release defaults.
            MIL.MbufFree(MilImageBuf);

            // Free buffers to leave space for possible temporary buffers.
            for (int n = 0; n < MilGrabBufferListSize; n++)
            {
                MIL.MbufFree(MilGrabBufferList[n]);
            }

            MIL.MdispFree(MilDisplay);
            MIL.MdigFree(MilDigitizer);

            return 0;
        }
    }
}
