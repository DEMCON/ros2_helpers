using std_msgs.msg;
using System;

namespace ROS2
{
    public interface IRos2Helper
    {
        public void InitializeRos(string transformRootName, string name, int gameObjectInstanceId);

        public Header CreateHeader(string frameId);

        public void CancelSpin();

        public Subscription<T>? CreateSubscription<T>(string topic, Action<T> callback, QosProfile? qosProfile = null)
            where T : IRosMessage, new();

        public Publisher<T>? CreatePublisher<T>(string topic, QosProfile? qosProfile = null)
            where T : IRosMessage;

        public Service<TService, TRequest, TResponse>? CreateService<TService, TRequest, TResponse>(
            string serviceName,
            Action<TRequest, TResponse> callback)
            where TService : IRosServiceDefinition<TRequest, TResponse>
            where TRequest : IRosMessage, new()
            where TResponse : IRosMessage, new();

        public Client<TService, TRequest, TResponse>? CreateClient<TService, TRequest, TResponse>(
            string serviceName)
            where TService : IRosServiceDefinition<TRequest, TResponse>
            where TRequest : IRosMessage, new()
            where TResponse : IRosMessage, new();

        public Timer? CreateTimer(TimeSpan period, Action<TimeSpan> callback);

        public GuardCondition? CreateGuardCondition(Action callback);

        public ActionClient<TAction, TGoal, TResult, TFeedback>? CreateActionClient<TAction, TGoal, TResult, TFeedback>(
            string actionName)
            where TAction : IRosActionDefinition<TGoal, TResult, TFeedback>
            where TGoal : IRosMessage, new()
            where TResult : IRosMessage, new()
            where TFeedback : IRosMessage, new();

        public ActionServer<TAction, TGoal, TResult, TFeedback>? CreateActionServer<TAction, TGoal, TResult, TFeedback>(
            string actionName,
            Action<ActionServerGoalHandle<TAction, TGoal, TResult, TFeedback>> acceptedCallback,
            Func<Guid, TGoal, GoalResponse>? goalCallback = null,
            Func<ActionServerGoalHandle<TAction, TGoal, TResult, TFeedback>, CancelResponse>? cancelCallback = null)
            where TAction : IRosActionDefinition<TGoal, TResult, TFeedback>
            where TGoal : IRosMessage, new()
            where TResult : IRosMessage, new()
            where TFeedback : IRosMessage, new();

        public geometry_msgs.msg.Quaternion GetRosQuaternionFromUnity(float x, float y, float z, float w);
        public geometry_msgs.msg.Vector3 GetRosVectorFromUnity(float x, float y, float z);

        public float GetTickRate();
        public void SetTickRate(float tickRate);
    }
}
