using System.Runtime.Serialization;

namespace HospitalWeb.Services.Utility
{
    public enum LiqPayResponseStatus
    {
        [EnumMember(Value = "sandbox")]
        Sandbox,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "failure")]
        Failure,
        [EnumMember(Value = "reversed")]
        Reversed,
        [EnumMember(Value = "success")]
        Success,
        [EnumMember(Value = "3ds_verify")]
        _3DSVerify,
        [EnumMember(Value = "cvv_verify")]
        CVVVerify,
        [EnumMember(Value = "otp_verify")]
        OTPVerify,
        [EnumMember(Value = "receiver_verify")]
        ReceiverVerify,
        [EnumMember(Value = "sender_verify")]
        SenderVerify,
        [EnumMember(Value = "wait_reserve")]
        WaitReserve,
        [EnumMember(Value = "wait_secure")]
        WaitSecure
    }
}
