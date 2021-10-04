using System.Collections;
using Godot;
using Events;

namespace GameSystems
{
    static class SceneCamera2D
    {
        static Camera2D camera, previous = null;
        [Command("Toggles the 2D scene Camera")]
        static void Camera2D(Command args)
        {
            if (Node.IsInstanceValid(camera))
            {
                if (Node.IsInstanceValid(previous))
                    previous.Current = true;
                camera.QueueFree();
            }
            else
            {
                foreach (var cam in Scene.Current.FindChildren<Camera2D>())
                {
                    if (cam.Current)
                    {
                        previous = cam;
                        break;
                    }
                }

                camera = new Camera2D();
                Scene.Current.AddChild(camera);
                camera.Current = true;

                Coroutine.Start(() =>
                {
                    while (Node.IsInstanceValid(camera))
                    {
                        var delta = Time.frame_time;
                        float speed = 128;
                        if (fast.pressed)
                            speed *= 20f;
                        if (slow.pressed)
                            speed *= .2f;
                        if (zoom_in.pressed)
                            camera.Zoom -= Vector2.One * delta * (speed / 100f);
                        if (zoom_out.pressed)
                            camera.Zoom += Vector2.One * delta * (speed / 100f);

                        Vector2 moveVector = new Vector2(left - right, up - down) * speed;
                        velocity = velocity.lerp(moveVector * Time.frame_time, Time.frame_time * 10f);
                        camera.Translate(velocity);
                        return true;
                    }
                    return false;
                });
            }
        }

        static InputAction up = new InputAction(Inputs.key_w, Inputs.key_up_arrow);
        static InputAction down = new InputAction(Inputs.key_s, Inputs.key_down_arrow);
        static InputAction left = new InputAction(Inputs.key_a, Inputs.key_left_arrow);
        static InputAction right = new InputAction(Inputs.key_d, Inputs.key_right_arrow);
        static InputAction zoom_in = new InputAction(Inputs.key_e, Inputs.key_pad_plus);
        static InputAction zoom_out = new InputAction(Inputs.key_q, Inputs.key_pad_minus);
        static InputAction slow = new InputAction(Inputs.key_shift);
        static InputAction fast = new InputAction(Inputs.key_space);

        static Vector2 velocity = Vector2.Zero;
    }

    static class SceneCamera3D
    {
        public static bool TryGet(out Camera scene_camera)
        {
            scene_camera = camera;
            return scene_camera.IsValid();
        }

        [Command]
        public static void Camera3D(Command args)
        {
            if (Node.IsInstanceValid(camera))
            {
                camera.QueueFree();
                if (Node.IsInstanceValid(previous_camera))
                    previous_camera.Current = true;
            }
            else
            {
                camera = new Camera();
                Scene.Current.AddChild(camera);
                camera.Current = true;

                Coroutine.Start(() =>
                {
                    float delta = Time.frame_time;
                    float speed = 5;
                    if (fast.pressed)
                        speed *= 10f;
                    if (slow.pressed)
                        speed *= .2f;
                    Vector3 moveVector = new Vector3(left - right, up - down, forward - back) * speed;
                    yaw += (look_left - look_right) * 3f * Time.frame_time;
                    pitch += (look_up - look_down) * 3f * Time.frame_time;

                    camera.Transform = new Transform(new Basis(new Vector3(pitch, yaw, 0)), camera.Transform.origin);
                    velocity = velocity.lerp(-moveVector * Time.frame_time, Time.frame_time * 10f);
                    camera.Translate(velocity);
                    return camera.IsValid();
                });
            }
        }

        static Camera camera, previous_camera;
        static InputAction forward = new InputAction(Inputs.key_w, Inputs.key_up_arrow);
        static InputAction back = new InputAction(Inputs.key_s, Inputs.key_down_arrow);
        static InputAction left = new InputAction(Inputs.key_a, Inputs.key_left_arrow);
        static InputAction right = new InputAction(Inputs.key_d, Inputs.key_right_arrow);
        static InputAction down = new InputAction(Inputs.key_e, Inputs.key_pad_plus);
        static InputAction up = new InputAction(Inputs.key_q, Inputs.key_pad_minus);
        static InputAction slow = new InputAction(Inputs.key_shift);
        static InputAction fast = new InputAction(Inputs.key_space);
        static InputAction look_left = new InputAction(Inputs.mouse_move_left, Inputs.key_pad_4);
        static InputAction look_right = new InputAction(Inputs.mouse_move_right, Inputs.key_pad_6);
        static InputAction look_down = new InputAction(Inputs.mouse_move_down, Inputs.key_pad_8);
        static InputAction look_up = new InputAction(Inputs.mouse_move_up, Inputs.key_pad_5);

        static Vector3 velocity = Vector3.Zero;
        static float yaw = 0, pitch = 0;
    }
}