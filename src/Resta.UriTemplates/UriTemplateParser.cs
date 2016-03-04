using System;
using System.Collections.Generic;
using System.Text;

namespace Resta.UriTemplates
{
    internal class UriTemplateParser
    {
        private readonly string _template;
        private StringBuilder _builder;
        private List<IUriComponent> _components;
        private Operator _exprOperator;
        private int _position;
        private Token _token;
        private bool _varSpecExloded;
        private int _varSpecMaxLength;
        private List<VarSpec> _varSpecs;

        public UriTemplateParser(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            _template = template;
        }

        public static List<IUriComponent> Parse(string template)
        {
            return new UriTemplateParser(template).Parse();
        }

        private List<IUriComponent> Parse()
        {
            try
            {
                _builder = new StringBuilder();
                _components = new List<IUriComponent>();
                _token = Token.Literal;

                for (_position = 0; _position < _template.Length; _position++)
                {
                    var ch = _template[_position];

                    switch (_token)
                    {
                        case Token.Literal:
                            ReadLiteral(ch);
                            break;

                        case Token.Expression:
                            ReadExpression(ch);
                            break;

                        case Token.VarSpec:
                            ReadVarSpec(ch);
                            break;

                        case Token.VarSpecExploded:
                            ReadVarSpecExploded(ch);
                            break;

                        case Token.VarSpecMaxLength:
                            ReadVarSpecMaxLength(ch);
                            break;
                    }
                }

                if (_token != Token.Literal)
                {
                    ThrowException("Unexpected end of URI template");
                }

                CreateLiteral();
            }
            catch (UriTemplateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UriTemplateParseException(
                    string.Format("Error at parse URI template: {0}", exception.Message), _template, _position,
                    exception);
            }

            return _components;
        }

        private void ReadLiteral(char ch)
        {
            if (ch == '{')
            {
                _token = Token.Expression;
            }
            else if (ch == '}')
            {
                ThrowException("Invalid literal character \"}\"");
            }
            else
            {
                _builder.Append(ch);
            }
        }

        private void ReadExpression(char ch)
        {
            CreateLiteral();

            _token = Token.VarSpec;

            if (!Operator.TryParse(ch, out _exprOperator))
            {
                _exprOperator = Operator.Default;
                ReadVarSpec(ch);
            }
        }

        private void ReadVarSpec(char ch)
        {
            if (TryReadEndVarSpec(ch))
            {
                return;
            }

            switch (ch)
            {
                case '*':
                    _token = Token.VarSpecExploded;
                    break;
                case ':':
                    _varSpecMaxLength = -1;
                    _token = Token.VarSpecMaxLength;
                    break;
                default:
                    if (!CharSpec.IsVarChar(ch))
                    {
                        ThrowException("Invalid name of template variable.");
                    }
                    else
                    {
                        _builder.Append(ch);
                    }
                    break;
            }
        }

        private void ReadVarSpecMaxLength(char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                var digit = ch - '0';

                if (_varSpecMaxLength == -1)
                {
                    _varSpecMaxLength = digit;
                }
                else
                {
                    _varSpecMaxLength = _varSpecMaxLength*10 + digit;
                }
            }
            else if (!TryReadEndVarSpec(ch))
            {
                ThrowException("Invalid URI template length modifier");
            }
        }

        private void ReadVarSpecExploded(char ch)
        {
            _varSpecExloded = true;

            if (!TryReadEndVarSpec(ch))
            {
                ThrowException("Invalid URI template modifier");
            }
        }

        private bool TryReadEndVarSpec(char ch)
        {
            if (ch == '}' || ch == ',')
            {
                CreateVarSpec();

                if (ch == '}')
                {
                    CreateExpression();
                    _token = Token.Literal;
                }
                else
                {
                    _token = Token.VarSpec;
                }

                return true;
            }

            return false;
        }

        private void CreateExpression()
        {
            var expr = new Expression(_exprOperator, _varSpecs);
            _components.Add(expr);
            _varSpecs = null;
        }

        private void CreateVarSpec()
        {
            if (_varSpecMaxLength == -1)
            {
                ThrowException("Invalid URI template modifier.");
            }

            var name = _builder.ToString();
            var varSpec = new VarSpec(name, _varSpecExloded, _varSpecMaxLength, true);

            if (_varSpecs == null)
            {
                _varSpecs = new List<VarSpec>();
            }

            _varSpecs.Add(varSpec);

            _builder.Length = 0;
            _varSpecExloded = false;
            _varSpecMaxLength = 0;
        }

        private void CreateLiteral()
        {
            if (_builder.Length != 0)
            {
                _components.Add(new Literal(_builder.ToString()));
                _builder.Length = 0;
            }
        }

        private void ThrowException(string message)
        {
            throw new UriTemplateParseException(message, _template, _position);
        }

        private enum Token
        {
            Literal,
            Expression,
            VarSpec,
            VarSpecMaxLength,
            VarSpecExploded
        }
    }
}