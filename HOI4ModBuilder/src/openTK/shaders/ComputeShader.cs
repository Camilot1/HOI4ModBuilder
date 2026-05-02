using OpenTK.Graphics.OpenGL;
using System;

namespace HOI4ModBuilder.src.openTK
{
    class ComputeShader : IDisposable
    {
        private readonly int _programId;
        private readonly int _shaderId;

        public ComputeShader(string shaderPath)
        {
            _shaderId = CreateShader(shaderPath);
            _programId = GL.CreateProgram();

            GL.AttachShader(_programId, _shaderId);
            GL.LinkProgram(_programId);
            GL.ValidateProgram(_programId);

            GL.GetProgram(_programId, ProgramParameter.LinkStatus, out int code);
            if (code == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_programId);
                throw new Exception($"Ошибка линковки compute shader программы ID = {_programId} \n\n {infoLog}");
            }

            GL.DetachShader(_programId, _shaderId);
            GL.DeleteShader(_shaderId);
        }

        public static bool IsSupported()
        {
            try
            {
                GL.GetInteger(GetPName.MajorVersion, out int majorVersion);
                GL.GetInteger(GetPName.MinorVersion, out int minorVersion);

                if (majorVersion > 4)
                    return true;

                if (majorVersion == 4 && minorVersion >= 3)
                    return true;
            }
            catch
            {
            }

            string version = GL.GetString(StringName.Version);
            if (string.IsNullOrWhiteSpace(version))
                return false;

            var versionToken = version.Split(' ')[0];
            if (!Version.TryParse(versionToken, out Version parsedVersion))
                return false;

            return parsedVersion.Major > 4 || parsedVersion.Major == 4 && parsedVersion.Minor >= 3;
        }

        public void ActiveProgram() => GL.UseProgram(_programId);

        public void DeactiveProgram() => GL.UseProgram(0);

        public void SetUniform1(string name, int value)
        {
            int location = GL.GetUniformLocation(_programId, name);
            GL.Uniform1(location, value);
        }

        public void Dispatch(int groupsX, int groupsY, int groupsZ, MemoryBarrierFlags barrierFlags)
        {
            GL.DispatchCompute(groupsX, groupsY, groupsZ);
            GL.MemoryBarrier(barrierFlags);
        }

        private static int CreateShader(string shaderPath)
        {
            string shaderText = ShaderSourceLoader.Load(shaderPath);
            int shaderId = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(shaderId, shaderText);
            GL.CompileShader(shaderId);

            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int code);
            if (code == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderId);
                throw new Exception($"Ошибка прикомпиляции compute shader ID = {shaderId} \n\n {infoLog}");
            }

            return shaderId;
        }

        public void Dispose()
        {
            GL.DeleteProgram(_programId);
        }
    }
}
