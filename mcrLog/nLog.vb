Imports NLog

Public Class NLog
    Private Shared nLogger As Logger

    Private Shared _bMsgBoxError As Boolean = False

    Private Shared iCountErrors As Integer
    Private Shared sLastError As String

    Private Shared ReadOnly lstLog As New List(Of CLogInfo)

    Shared Function LogConfig(ByVal bMsgBoxError As Boolean) As Boolean
        Try

            nLogger = LogManager.GetCurrentClassLogger()

            _bMsgBoxError = bMsgBoxError

            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

    Shared Function LogGetNumErrors() As Integer
        Try
            Return iCountErrors

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "logGetNumErrors Exception")
            End If
            Return False
        End Try
    End Function

    Shared Function LogGetListLog(ByRef oList As List(Of mcrLog.CLogInfo)) As Boolean
        Try
            oList = lstLog.ToList
            Return True

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "logGetNumErrors Exception")
            End If
            Return False
        End Try
    End Function

    Shared Function LogGetLastError() As String
        Try
            Return sLastError

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "logGetLastError Exception")
            End If
            Return "...."
        End Try
    End Function

    Shared Sub LogResetError()
        Try
            sLastError = ""
            iCountErrors = 0

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogResetError Exception")
            End If
        End Try
    End Sub


    Shared Function LogDecodeLevel(ByVal iLevel As Integer) As String
        Try
            Select Case iLevel
                Case 0
                    Return "Trace"
                Case 1
                    Return "Debug"
                Case 2
                    Return "Info"
                Case 3
                    Return "Warning"
                Case 4
                    Return "Error"
                Case 5
                    Return "Fatal"
                Case Else
                    Return "Unknown"
            End Select
        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogDecodeLevel")
            End If
            Return "MsgErr"
        End Try
    End Function

    Shared Sub GetConfigOptions(ByRef bTrace As Boolean, ByRef bDebug As Boolean, ByRef bInfo As Boolean, ByRef bWarning As Boolean, ByRef bError As Boolean, ByRef bFatal As Boolean)
        Try
            bTrace = nLogger.IsTraceEnabled
            bDebug = nLogger.IsDebugEnabled
            bInfo = nLogger.IsInfoEnabled

            bWarning = nLogger.IsWarnEnabled
            bError = nLogger.IsErrorEnabled
            bFatal = nLogger.IsFatalEnabled

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "GetConfigOptions Exception")
            End If
        End Try
    End Sub

    Shared Sub LogTrace(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String)
        Dim sLogMessage As String
        Try

            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.ilogTrace, sApp, sModule, sFunction, sMessage, "")
            sLogMessage += sMessage

            nLogger.Trace(sLogMessage)

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogTrace Exception")
            End If
        End Try
    End Sub

    Shared Sub LogDebug(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String)
        Dim sLogMessage As String
        Try
            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.ilogDebug, sApp, sModule, sFunction, sMessage, "")
            sLogMessage += sMessage

            nLogger.Debug(sLogMessage)

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogDebug Exception")
            End If
        End Try
    End Sub

    Shared Sub LogInfo(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String)
        Dim sLogMessage As String
        Try
            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.ilogInfo, sApp, sModule, sFunction, sMessage, "")
            sLogMessage += sMessage

            nLogger.Info(" " + sLogMessage)

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogInfo Exception")
            End If
        End Try
    End Sub

    Shared Sub LogWarning(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String)
        Dim sLogMessage As String
        Try

            iCountErrors += 1

            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.ilogWarning, sApp, sModule, sFunction, sMessage, "")
            sLogMessage += sMessage

            sLastError = String.Format("WAR {0:MM/dd/yy HH:mm:ss zzz}  - {1}", Date.Now, sLogMessage)

            nLogger.Warn(" " + sLogMessage)

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogWarning Exception")
            End If
        End Try
    End Sub

    Shared Sub LogError(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal eMessage As Exception, Optional ByVal sMessage As String = "")
        Dim sLogMessage As String
        Try

            iCountErrors += 1

            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.iLogError, sApp, sModule, sFunction, sMessage, eMessage.Message)
            If sMessage.Length > 0 Then
                sLogMessage = sLogMessage + sMessage + " " + eMessage.Message
            Else
                sLogMessage += eMessage.Message
            End If

            nLogger.Error(sLogMessage)
            nLogger.Error(eMessage)

            sLastError = String.Format("ERR {0:MM/dd/yy HH:mm:ss zzz}  - {1}", Date.Now, sLogMessage)

            If _bMsgBoxError Then
                MsgBox(sLogMessage & vbCrLf & vbCrLf & eMessage.ToString, MsgBoxStyle.Critical, "LogError")
            End If

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogError Exception")
            End If
        End Try
    End Sub

    Shared Sub LogFatal(ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal eMessage As Exception, Optional ByVal sMessage As String = "")
        Dim sLogMessage As String
        Try

            iCountErrors += 1

            sLogMessage = LogCreateMessage(CLogInfo.LogLevel.iLogFatal, sApp, sModule, sFunction, sMessage, eMessage.Message)
            If sMessage.Length > 0 Then
                sLogMessage = sLogMessage + sMessage + " " + eMessage.Message
            Else
                sLogMessage += eMessage.Message
            End If

            nLogger.Fatal(sLogMessage)
            nLogger.Fatal(eMessage)

            sLastError = String.Format("FAT {0:MM/dd/yy HH:mm:ss zzz}  - {1}", Date.Now, sLogMessage)

            If _bMsgBoxError Then
                MsgBox(sLogMessage & vbCrLf & vbCrLf & eMessage.ToString, MsgBoxStyle.Critical, "LogFatal")
            End If

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogFatal Exception")
            End If
        End Try
    End Sub

    Private Shared Function LogCreateMessage(ByVal iloglevel As CLogInfo.LogLevel, ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String, ByVal sEx As String) As String
        Dim sMessageDisplay As String '= ""

        Try
            sMessageDisplay = ""
            sMessageDisplay = sMessageDisplay + (sApp + "                       ").Substring(0, 15) + " "
            sMessageDisplay = sMessageDisplay + (sModule + "                    ").Substring(0, 20) + " "
            sMessageDisplay = sMessageDisplay + (sFunction + "                                        ").Substring(0, 40) + " "

            AddLogListInfo(iloglevel, sApp, sModule, sFunction, sMessage, sEx)

            Return sMessageDisplay

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "logCreateMessage Exception")
            End If
            Return "logCreateMessage Exception"
        End Try
    End Function

    Private Shared Sub AddLogListInfo(ByVal iloglevel As CLogInfo.LogLevel, ByVal sApp As String, ByVal sModule As String, ByVal sFunction As String, ByVal sMessage As String, ByVal sEx As String)
        Dim oItemLog As New CLogInfo

        Try
            oItemLog.Level = iloglevel
            oItemLog.AppLog = sApp
            oItemLog.ModuleLog = sModule
            oItemLog.FunctionLog = sFunction
            oItemLog.MessageLog = sMessage
            oItemLog.ExLog = sEx

            lstLog.Add(oItemLog)

            If lstLog.Count > 2000 Then
                lstLog.RemoveRange(0, 100)
            End If

        Catch ex As Exception
            If _bMsgBoxError Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "AddLogListInfo Exception")
            End If
        End Try
    End Sub

End Class

Public Class CLogInfo

#Region "Data"
    'Private Const sModuleName As String = "Soap-cLogInfo"

    Public Enum LogLevel
        ilogTrace = 0
        ilogDebug = 1
        ilogInfo = 2
        ilogWarning = 3
        iLogError = 4
        iLogFatal = 5
    End Enum

    Public Property DataLog As String

    Public Property Level As logLevel
    Public Property AppLog As String
    Public Property ModuleLog As String
    Public Property FunctionLog As String
    Public Property MessageLog As String
    Public Property ExLog As String

#End Region

    Public Sub New()

        Try
            DataLog = String.Format("{0:dd/MM/yyyy HH:mm:ss fff}", Date.Now)
            Level = -1
            AppLog = ""
            ModuleLog = ""
            FunctionLog = ""
            MessageLog = ""
            ExLog = -""

        Catch ex As Exception
            If False Then
                MsgBox(ex.ToString, MsgBoxStyle.Critical, "LogList New Exception")
            End If
        End Try

    End Sub

End Class
