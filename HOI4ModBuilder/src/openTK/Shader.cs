using HOI4ModBuilder.src.utils;
using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace HOI4ModBuilder.src.openTK
{
    class Shader
    {
        private readonly int programID = 0;
        private readonly int vertexShaderID = 0;
        private readonly int fragmentShaderID = 0;

        public Shader(string vertexFile, string fragmentFile)
        {
            vertexShaderID = CreateShader(ShaderType.VertexShader, vertexFile);
            fragmentShaderID = CreateShader(ShaderType.FragmentShader, fragmentFile);

            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);

            GL.LinkProgram(programID);

            //CHECK. Если убрать, то будет
            //"OpenGL debug message: Program/shader state performance warning: Vertex shader in program 3 is being recompiled based on GL state, and was not found in the disk cache"
            GL.ValidateProgram(programID);

            GL.GetProgram(programID, ProgramParameter.LinkStatus, out int code);

            if (code != (int)All.True)
            {
                var infoLog = GL.GetProgramInfoLog(programID);
                throw new Exception($"Ошибка линковки шейдерной программы ID = {programID} \n\n {infoLog}");
            }

            DeleteShader(vertexShaderID);
            DeleteShader(fragmentShaderID);
        }

        public void ActiveProgram() => GL.UseProgram(programID);

        public void DeactiveProgram() => GL.UseProgram(0);

        public void DeleteProgram() => GL.DeleteProgram(programID);

        public int GetAttribProgram(string name) => GL.GetAttribLocation(programID, name);

        public void SetUniform4(string name, Vector4 vec)
        {
            int location = GL.GetUniformLocation(programID, name);
            GL.Uniform4(location, vec);
        }

        private int CreateShader(ShaderType type, string file)
        {
            string shaderText = File.ReadAllText(file);
            int shaderID = GL.CreateShader(type);
            GL.ShaderSource(shaderID, shaderText);
            GL.CompileShader(shaderID);

            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out int code);

            if (code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shaderID);
                throw new Exception($"Ошибка прикомпиляции шейдера ID = {shaderID} \n\n {infoLog}");
            }

            return shaderID;
        }

        private void DeleteShader(int shaderID)
        {
            GL.DeleteShader(shaderID);
            GL.DeleteShader(shaderID);
        }
    }
}
