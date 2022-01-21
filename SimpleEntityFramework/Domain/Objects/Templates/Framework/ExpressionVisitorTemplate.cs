using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEntityFramework.Domain.Objects.Templates.Framework
{
    public class ExpressionVisitorTemplate : ClassTemplate
    {
        public const string ClassName = "ExpressionVisitor";

        public override string Name => ClassName;

        public override string FileName => $"{Name}`1.cs";

        public override string FileContent => $@"{Profile}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace {Namespace}
{{
    public class {Name}<TEntity> : ExpressionVisitor
    {{
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbProviderFactory _dbProviderFactory;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _groupBy = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _orderBy = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<DbParameter> _parameters = new List<DbParameter>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _select = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _update = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _where = new List<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? _skip;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int? _take;

        public {Name}(DbProviderFactory dbProviderFactory, Expression expression)
        {{
            _dbProviderFactory = dbProviderFactory;
            Visit(expression);
            TableName = typeof(TEntity).Name;
        }}

        public string CommandText => string.Join(Environment.NewLine, BuildSqlStatement());

        public string From => $""FROM [{{TableName}}]"";
        public string GroupBy => _groupBy.Count == 0 ? null : ""GROUP BY "" + string.Join("", "", _groupBy);
        public bool IsDelete {{ get; private set; }} = false;
        public bool IsDistinct {{ get; private set; }}
        public string OrderBy => string.Join("" "", BuildOrderByStatement());
        public DbParameter[] Parameters => _parameters.ToArray();
        public string Select => string.Join("" "", BuildSelectStatement());
        public int? Skip => _skip;
        public string TableName {{ get; private set; }}
        public int? Take => _take;
        public string Update => ""SET "" + string.Join("", "", _update);
        public string Where => _where.Count == 0 ? null : ""WHERE "" + string.Join("" "", _where);

        public static implicit operator string({Name}<TEntity> visitor) => visitor.ToString();

        public override string ToString() => string.Join(Environment.NewLine, BuildSqlStatement());

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {{
            _where.Add(""("");
            Visit(binaryExpression.Left);

            switch (binaryExpression.NodeType)
            {{
                case ExpressionType.And:
                    _where.Add(""AND"");
                    break;

                case ExpressionType.AndAlso:
                    _where.Add(""AND"");
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _where.Add(""OR"");
                    break;

                case ExpressionType.Equal:
                    if (IsNullConstant(binaryExpression.Right))
                    {{
                        _where.Add(""IS"");
                    }}
                    else
                    {{
                        _where.Add(""="");
                    }}
                    break;

                case ExpressionType.NotEqual:
                    if (IsNullConstant(binaryExpression.Right))
                    {{
                        _where.Add(""IS NOT"");
                    }}
                    else
                    {{
                        _where.Add(""<>"");
                    }}
                    break;

                case ExpressionType.LessThan:
                    _where.Add(""<"");
                    break;

                case ExpressionType.LessThanOrEqual:
                    _where.Add(""<="");
                    break;

                case ExpressionType.GreaterThan:
                    _where.Add("">"");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    _where.Add("">="");
                    break;

                default:
                    throw new NotSupportedException($""The binary operator '{{binaryExpression.NodeType}}' is not supported"");
            }}

            Visit(binaryExpression.Right);
            _where.Add("")"");
            return binaryExpression;
        }}

        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {{
            switch (constantExpression.Value)
            {{
                case null when constantExpression.Value == null:
                    _where.Add(""NULL"");
                    break;

                default:

                    _where.Add(CreateParameter(constantExpression.Value).ParameterName);

                    break;
            }}

            return constantExpression;
        }}

        protected override Expression VisitMember(MemberExpression memberExpression)
        {{
            Expression VisitMemberLocal(Expression expression)
            {{
                switch (expression.NodeType)
                {{
                    case ExpressionType.Parameter:
                        _where.Add($""[{{memberExpression.Member.Name}}]"");
                        return memberExpression;

                    case ExpressionType.Constant:
                        _where.Add(CreateParameter(GetValue(memberExpression)).ParameterName);

                        return memberExpression;

                    case ExpressionType.MemberAccess:
                        _where.Add(CreateParameter(GetValue(memberExpression)).ParameterName);

                        return memberExpression;
                }}

                throw new NotSupportedException($""The member '{{memberExpression.Member.Name}}' is not supported"");
            }}

            if (memberExpression.Expression == null)
            {{
                return VisitMemberLocal(memberExpression);
            }}

            return VisitMemberLocal(memberExpression.Expression);
        }}

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {{
            switch (methodCallExpression.Method.Name)
            {{
                case nameof(Queryable.Where) when methodCallExpression.Method.DeclaringType == typeof(Queryable):

                    Visit(methodCallExpression.Arguments[0]);
                    var lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);
                    Visit(lambda.Body);

                    return methodCallExpression;

                case nameof(Queryable.Select):
                    return ParseExpression(methodCallExpression, _select);

                case nameof(Queryable.GroupBy):
                    return ParseExpression(methodCallExpression, _groupBy);

                case nameof(Queryable.Take):
                    return ParseExpression(methodCallExpression, ref _take);

                case nameof(Queryable.Skip):
                    return ParseExpression(methodCallExpression, ref _skip);

                case nameof(Queryable.OrderBy):
                case nameof(Queryable.ThenBy):
                    return ParseExpression(methodCallExpression, _orderBy, ""ASC"");

                case nameof(Queryable.OrderByDescending):
                case nameof(Queryable.ThenByDescending):
                    return ParseExpression(methodCallExpression, _orderBy, ""DESC"");

                case nameof(Queryable.Distinct):
                    IsDistinct = true;
                    return Visit(methodCallExpression.Arguments[0]);

                case nameof(string.StartsWith):
                    _where.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _where.Add(""LIKE"");
                    _where.Add(CreateParameter(GetValue(methodCallExpression.Arguments[0]).ToString() + ""%"").ParameterName);
                    return methodCallExpression.Arguments[0];

                case nameof(string.EndsWith):
                    _where.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _where.Add(""LIKE"");
                    _where.Add(CreateParameter(""%"" + GetValue(methodCallExpression.Arguments[0]).ToString()).ParameterName);
                    return methodCallExpression.Arguments[0];

                case nameof(string.Contains):
                    _where.AddRange(ParseExpression(methodCallExpression, methodCallExpression.Object));
                    _where.Add(""LIKE"");
                    _where.Add(CreateParameter(""%"" + GetValue(methodCallExpression.Arguments[0]).ToString() + ""%"").ParameterName);
                    return methodCallExpression.Arguments[0];

                case ""ToSqlString""/*nameof(Extensions.ToSqlString)*/:
                    return Visit(methodCallExpression.Arguments[0]);

                case ""Delete""/*nameof(Extensions.Delete)*/:
                case ""DeleteAsync""/*nameof(Extensions.DeleteAsync)*/:
                    IsDelete = true;
                    return Visit(methodCallExpression.Arguments[0]);

                case ""Update""/*nameof(Extensions.Update)*/:
                    return ParseExpression(methodCallExpression, _update);

                default:
                    if (methodCallExpression.Object != null)
                    {{
                        _where.Add(CreateParameter(GetValue(methodCallExpression)).ParameterName);
                        return methodCallExpression;
                    }}
                    break;
            }}

            throw new NotSupportedException($""The method '{{methodCallExpression.Method.Name}}' is not supported"");
        }}

        protected override Expression VisitUnary(UnaryExpression unaryExpression)
        {{
            switch (unaryExpression.NodeType)
            {{
                case ExpressionType.Not:
                    _where.Add(""NOT"");
                    Visit(unaryExpression.Operand);
                    break;

                case ExpressionType.Convert:
                    Visit(unaryExpression.Operand);
                    break;

                default:
                    throw new NotSupportedException($""The unary operator '{{unaryExpression.NodeType}}' is not supported"");
            }}
            return unaryExpression;
        }}

        private static Expression StripQuotes(Expression expression)
        {{
            while (expression.NodeType == ExpressionType.Quote)
            {{
                expression = ((UnaryExpression)expression).Operand;
            }}
            return expression;
        }}

        //[SuppressMessage(""Style"", ""IDE0011:Add braces"", Justification = ""Easier to read"")]
        //private IEnumerable<string> BuildDeclaration()
        //{{
        //    if (Parameters.Length == 0)                        /**/    yield break;
        //    foreach (DbParameter parameter in Parameters)     /**/    yield return $""DECLARE {{parameter.ParameterName}} {{parameter.SqlDbType}}"";

        //    foreach (DbParameter parameter in Parameters)     /**/
        //        if (parameter.SqlDbType.RequiresQuotes())      /**/    yield return $""SET {{parameter.ParameterName}} = '{{parameter.SqlValue?.ToString().Replace(""'"", ""''"") ?? ""NULL""}}'"";
        //        else                                           /**/    yield return $""SET {{parameter.ParameterName}} = {{parameter.SqlValue}}"";
        //}}

        [SuppressMessage(""Style"", ""IDE0011:Add braces"", Justification = ""Easier to read"")]
        private IEnumerable<string> BuildOrderByStatement()
        {{
            if (Skip.HasValue && _orderBy.Count == 0)                       /**/   yield return ""ORDER BY (SELECT NULL)"";
            else if (_orderBy.Count == 0)                                   /**/   yield break;
            else if (_groupBy.Count > 0 && _orderBy[0].StartsWith(""[Key]"")) /**/   yield return ""ORDER BY "" + string.Join("", "", _groupBy);
            else                                                            /**/   yield return ""ORDER BY "" + string.Join("", "", _orderBy);

            if (Skip.HasValue && Take.HasValue)                             /**/   yield return $""OFFSET {{Skip}} ROWS FETCH NEXT {{Take}} ROWS ONLY"";
            else if (Skip.HasValue && !Take.HasValue)                       /**/   yield return $""OFFSET {{Skip}} ROWS"";
        }}

        [SuppressMessage(""Style"", ""IDE0011:Add braces"", Justification = ""Easier to read"")]
        private IEnumerable<string> BuildSelectStatement()
        {{
            yield return ""SELECT"";

            if (IsDistinct)                                 /**/    yield return ""DISTINCT"";

            if (Take.HasValue && !Skip.HasValue)            /**/    yield return $""TOP ({{Take.Value}})"";

            if (_select.Count == 0 && _groupBy.Count > 0)   /**/    yield return string.Join("", "", _groupBy.Select(x => $""MAX({{x}})""));
            else if (_select.Count == 0)                    /**/    yield return ""*"";
            else                                            /**/    yield return string.Join("", "", _select);
        }}

        [SuppressMessage(""Style"", ""IDE0011:Add braces"", Justification = ""Easier to read"")]
        private IEnumerable<string> BuildSqlStatement()
        {{
            if (IsDelete)                   /**/   yield return ""DELETE"";
            else if (_update.Count > 0)     /**/   yield return $""UPDATE [{{TableName}}]"";
            else                            /**/   yield return Select;

            if (_update.Count == 0)         /**/   yield return From;
            else if (_update.Count > 0)     /**/   yield return Update;

            if (Where != null)              /**/   yield return Where;
            if (GroupBy != null)            /**/   yield return GroupBy;
            if (OrderBy != null)            /**/   yield return OrderBy;
        }}

        private DbParameter CreateParameter(object value)
        {{
            string parameterName = $""@p{{_parameters.Count}}"";

            var parameter = _dbProviderFactory.CreateParameter();
            {{
                parameter.Direction = ParameterDirection.Input;
                parameter.ParameterName = parameterName;
                parameter.Value = value;
                var converter = TypeDescriptor.GetConverter(parameter.DbType);
                {{
                    var type = value.GetType();
                    if (converter.CanConvertFrom(type))
                    {{
                        parameter.DbType = (DbType)converter.ConvertFrom(type.Name);
                    }}
                    else
                    {{
                        try
                        {{
                            parameter.DbType = (DbType)converter.ConvertFrom(type.Name);
                        }}
                        catch {{ }}
                    }}
                }}
            }}

            _parameters.Add(parameter);

            return parameter;
        }}

        private IEnumerable<string> GetNewExpressionString(NewExpression newExpression, string appendString = null)
        {{
            for (int i = 0; i < newExpression.Members.Count; i++)
            {{
                if (newExpression.Arguments[i].NodeType == ExpressionType.MemberAccess)
                {{
                    yield return
                        appendString == null ?
                        $""[{{newExpression.Members[i].Name}}]"" :
                        $""[{{newExpression.Members[i].Name}}] {{appendString}}"";
                }}
                else
                {{
                    yield return
                        appendString == null ?
                        $""[{{newExpression.Members[i].Name}}] = {{CreateParameter(GetValue(newExpression.Arguments[i])).ParameterName}}"" :
                        $""[{{newExpression.Members[i].Name}}] = {{CreateParameter(GetValue(newExpression.Arguments[i])).ParameterName}}"";
                }}
            }}
        }}

        private object GetValue(Expression expression)
        {{
            object GetMemberValue(MemberInfo memberInfo, object container = null)
            {{
                switch (memberInfo)
                {{
                    case FieldInfo fieldInfo:
                        return fieldInfo.GetValue(container);

                    case PropertyInfo propertyInfo:
                        return propertyInfo.GetValue(container);

                    default: return null;
                }}
            }}

            switch (expression)
            {{
                case ConstantExpression constantExpression:
                    return constantExpression.Value;

                case MemberExpression memberExpression when memberExpression.Expression is ConstantExpression constantExpression:
                    return GetMemberValue(memberExpression.Member, constantExpression.Value);

                case MemberExpression memberExpression when memberExpression.Expression is null: // static
                    return GetMemberValue(memberExpression.Member);

                case MethodCallExpression methodCallExpression:
                    return Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();

                case null:
                    return null;
            }}

            throw new NotSupportedException();
        }}

        private bool IsNullConstant(Expression expression) => expression.NodeType == ExpressionType.Constant && ((ConstantExpression)expression).Value == null;

        private IEnumerable<string> ParseExpression(Expression parent, Expression body, string appendString = null)
        {{
            switch (body)
            {{
                case MemberExpression memberExpression:
                    return appendString == null ?
                        new string[] {{ $""[{{memberExpression.Member.Name}}]"" }} :
                        new string[] {{ $""[{{memberExpression.Member.Name}}] {{appendString}}"" }};

                case NewExpression newExpression:
                    return GetNewExpressionString(newExpression, appendString);

                case ParameterExpression parameterExpression when parent is LambdaExpression lambdaExpression && lambdaExpression.ReturnType == parameterExpression.Type:
                    return new string[0];

                case ConstantExpression constantExpression:
                    return constantExpression
                        .Type
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(x => $""[{{x.Name}}] = {{CreateParameter(x.GetValue(constantExpression.Value)).ParameterName}}"");
            }}

            throw new NotSupportedException();
        }}

        private Expression ParseExpression(MethodCallExpression expression, List<string> commandList, string appendString = null)
        {{
            var unary = (UnaryExpression)expression.Arguments[1];
            var lambdaExpression = (LambdaExpression)unary.Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            commandList.AddRange(ParseExpression(lambdaExpression, lambdaExpression.Body, appendString));

            return Visit(expression.Arguments[0]);
        }}

        private Expression ParseExpression(MethodCallExpression expression, ref int? size)
        {{
            var sizeExpression = (ConstantExpression)expression.Arguments[1];

            if (int.TryParse(sizeExpression.Value.ToString(), out int value))
            {{
                size = value;
                return Visit(expression.Arguments[0]);
            }}

            throw new NotSupportedException();
        }}

        /// <summary>
        /// Enables the partial evaluation of queries.
        /// </summary>
        /// <remarks>
        /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
        /// Copyright notice http://msdn.microsoft.com/en-gb/cc300389.aspx#O
        /// </remarks>
        static class Evaluator
        {{
            /// <summary>
            /// Performs evaluation and replacement of independent sub-trees
            /// </summary>
            /// <param name=""expression"">The root of the expression tree.</param>
            /// <param name=""fnCanBeEvaluated"">A function that decides whether a given expression node can be part of the local function.</param>
            /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
            public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
            {{
                return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
            }}

            /// <summary>
            /// Performs evaluation and replacement of independent sub-trees
            /// </summary>
            /// <param name=""expression"">The root of the expression tree.</param>
            /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
            public static Expression PartialEval(Expression expression)
            {{
                return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
            }}

            private static bool CanBeEvaluatedLocally(Expression expression)
            {{
                return expression.NodeType != ExpressionType.Parameter;
            }}

            /// <summary>
            /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
            /// </summary>
            class SubtreeEvaluator : ExpressionVisitor
            {{
                HashSet<Expression> candidates;

                internal SubtreeEvaluator(HashSet<Expression> candidates)
                {{
                    this.candidates = candidates;
                }}

                internal Expression Eval(Expression exp)
                {{
                    return this.Visit(exp);
                }}

                public override Expression Visit(Expression exp)
                {{
                    if (exp == null)
                    {{
                        return null;
                    }}
                    if (this.candidates.Contains(exp))
                    {{
                        return this.Evaluate(exp);
                    }}
                    return base.Visit(exp);
                }}

                private Expression Evaluate(Expression e)
                {{
                    if (e.NodeType == ExpressionType.Constant)
                    {{
                        return e;
                    }}
                    LambdaExpression lambda = Expression.Lambda(e);
                    Delegate fn = lambda.Compile();
                    return Expression.Constant(fn.DynamicInvoke(null), e.Type);
                }}

                protected override Expression VisitMemberInit(MemberInitExpression node)
                {{
                    if (node.NewExpression.NodeType == ExpressionType.New)
                        return node;

                    return base.VisitMemberInit(node);
                }}
            }}

            /// <summary>
            /// Performs bottom-up analysis to determine which nodes can possibly
            /// be part of an evaluated sub-tree.
            /// </summary>
            class Nominator : ExpressionVisitor
            {{
                Func<Expression, bool> fnCanBeEvaluated;
                HashSet<Expression> candidates;
                bool cannotBeEvaluated;

                internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
                {{
                    this.fnCanBeEvaluated = fnCanBeEvaluated;
                }}

                internal HashSet<Expression> Nominate(Expression expression)
                {{
                    this.candidates = new HashSet<Expression>();
                    this.Visit(expression);
                    return this.candidates;
                }}

                public override Expression Visit(Expression expression)
                {{
                    if (expression != null)
                    {{
                        bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                        this.cannotBeEvaluated = false;
                        base.Visit(expression);
                        if (!this.cannotBeEvaluated)
                        {{
                            if (this.fnCanBeEvaluated(expression))
                            {{
                                this.candidates.Add(expression);
                            }}
                            else
                            {{
                                this.cannotBeEvaluated = true;
                            }}
                        }}
                        this.cannotBeEvaluated |= saveCannotBeEvaluated;
                    }}
                    return expression;
                }}
            }}
        }}
    }}
}}";
    }
}
