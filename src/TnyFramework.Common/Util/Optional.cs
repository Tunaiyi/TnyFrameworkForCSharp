// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

// using System;
// using System.Collections.Generic;
//
// namespace TnyFramework.Common.Util
// {
//
//     public class Optional
//     {
//         public static Optional<TElement> GetEmpty<TElement>() => Optional<TElement>.Empty;
//
//         /// <summary>
//         /// Returns an {@code Optional} describing the given non-{@code null}
//         /// value.
//         /// </summary>
//         /// @param value the value to describe, which must be non-{@code null}
//         /// @param <T> the type of the value
//         /// @return an {@code Optional} with the value present
//         /// @throws NullPointerException if value is {@code null}
//         public static Optional<TValue> Of<TValue>(TValue value)
//         {
//             return new Optional<TValue>(value);
//         }
//
//         /// <summary>
//         /// Returns an {@code Optional} describing the given value, if
//         /// non-{@code null}, otherwise returns an empty {@code Optional}.
//         /// </summary>
//         /// @param value the possibly-{@code null} value to describe
//         /// @param <T> the type of the value
//         /// @return an {@code Optional} with a present value if the specified value
//         ///         is non-{@code null}, otherwise an empty {@code Optional}
//         public static Optional<TValue> OfNullable<TValue>(TValue value)
//         {
//             return value == null ? Optional<TValue>.Empty : Of(value);
//         }
//     }
//
//     public class Optional<TValue> : Optional
//     {
//         public static Optional<TValue> Empty => new Optional<TValue>();
//
//         private readonly TValue? value;
//
//         private Optional()
//         {
//             value = default!;
//         }
//
//         internal Optional(TValue value)
//         {
//             if (value == null)
//             {
//                 throw new NullReferenceException();
//             }
//             this.value = value;
//         }
//
//         /// <summary>
//         /// If a value is present, returns the value, otherwise throws
//         /// {@code NoSuchElementException}.
//         /// </summary>
//         /// @apiNote
//         /// The preferred alternative to this method is {@link #orElseThrow()}.
//         /// </summary>
//         /// @return the non-{@code null} value described by this {@code Optional}
//         /// @throws NoSuchElementException if no value is present
//         public TValue Get()
//         {
//             if (value == null)
//             {
//                 throw new NullReferenceException("No value present");
//             }
//             return value;
//         }
//
//         /// <summary>
//         /// If a value is present, returns {@code true}, otherwise {@code false}.
//         /// </summary>
//         /// @return {@code true} if a value is present, otherwise {@code false}
//         public bool IsPresent()
//         {
//             return value != null;
//         }
//
//         /// <summary>
//         /// If a value is  not present, returns {@code true}, otherwise
//         /// {@code false}.
//         /// </summary>
//         /// @return  {@code true} if a value is not present, otherwise {@code false}
//         /// @since   11
//         public bool IsEmpty()
//         {
//             return value == null;
//         }
//
//         /// <summary>
//         /// If a value is present, performs the given action with the value,
//         /// otherwise does nothing.
//         /// </summary>
//         /// @param action the action to be performed, if a value is present
//         /// @throws NullPointerException if value is present and the given action is
//         ///         {@code null}
//         public void IfPresent(Action<TValue> action)
//         {
//             if (value != null)
//             {
//                 action(value);
//             }
//         }
//
//         /// <summary>
//         /// If a value is present, performs the given action with the value,
//         /// otherwise performs the given empty-based action.
//         /// </summary>
//         /// @param action the action to be performed, if a value is present
//         /// @param emptyAction the empty-based action to be performed, if no value is
//         ///        present
//         /// @throws NullPointerException if a value is present and the given action
//         ///         is {@code null}, or no value is present and the given empty-based
//         ///         action is {@code null}.
//         /// @since 9
//         public void IfPresentOrElse(Action<TValue> action, Action emptyAction)
//         {
//             if (value != null)
//             {
//                 action(value);
//             } else
//             {
//                 emptyAction();
//             }
//         }
//
//         /// <summary>
//         /// If a value is present, and the value matches the given predicate,
//         /// returns an {@code Optional} describing the value, otherwise returns an
//         /// empty {@code Optional}.
//         /// </summary>
//         /// @param predicate the predicate to apply to a value, if present
//         /// @return an {@code Optional} describing the value of this
//         ///         {@code Optional}, if a value is present and the value matches the
//         ///         given predicate, otherwise an empty {@code Optional}
//         /// @throws NullPointerException if the predicate is {@code null}
//         public Optional<TValue> Filter(Predicate<TValue> predicate)
//         {
//             if (!IsPresent())
//             {
//                 return this;
//             }
//             return predicate(value!) ? this : Empty;
//         }
//
//         public Optional<TOther> Map<TOther>(Func<TValue, TOther> mapper)
//         {
//             return !IsPresent() ? GetEmpty<TOther>() : OfNullable(mapper(value!));
//         }
//
//         /// <summary>
//         /// If a value is present, returns the result of applying the given
//         /// {@code Optional}-bearing mapping function to the value, otherwise returns
//         /// an empty {@code Optional}.
//         /// </summary>
//         /// This method is similar to {@link #map(Function)}, but the mapping
//         /// function is one whose result is already an {@code Optional}, and if
//         /// invoked, {@code flatMap} does not wrap it within an additional
//         /// {@code Optional}.
//         /// @param <U> The type of value of the {@code Optional} returned by the
//         ///            mapping function
//         /// @param mapper the mapping function to apply to a value, if present
//         /// @return the result of applying an {@code Optional}-bearing mapping
//         ///         function to the value of this {@code Optional}, if a value is
//         ///         present, otherwise an empty {@code Optional}
//         /// @throws NullPointerException if the mapping function is {@code null} or
//         ///         returns a {@code null} result
//         public Optional<TOther> FlatMap<TOther>(Func<TValue, Optional<TOther>> mapper)
//         {
//             if (!IsPresent())
//             {
//                 return Optional<TOther>.Empty;
//             }
//             var other = mapper(value!);
//             if (other == null)
//             {
//                 throw new NullReferenceException("FlatMap value is null");
//             }
//             return other;
//         }
//
//         /// <summary>
//         /// If a value is present, returns an {@code Optional} describing the value,
//         /// otherwise returns an {@code Optional} produced by the supplying function.
//         /// </summary>
//         /// @param supplier the supplying function that produces an {@code Optional}
//         ///        to be returned
//         /// @return returns an {@code Optional} describing the value of this
//         ///         {@code Optional}, if a value is present, otherwise an
//         ///         {@code Optional} produced by the supplying function.
//         /// @throws NullPointerException if the supplying function is {@code null} or
//         ///         produces a {@code null} result
//         /// @since 9
//         public Optional<TValue> Or(Func<Optional<TValue>> supplier)
//         {
//             if (IsPresent())
//             {
//                 return this;
//             }
//             var other = supplier();
//             if (other == null)
//             {
//                 throw new NullReferenceException("FlatMap value is null");
//             }
//             return other;
//         }
//
//         /// <summary>
//         /// If a value is present, returns a sequential {@link Stream} containing
//         /// only that value, otherwise returns an empty {@code Stream}.
//         /// </summary>
//         /// @apiNote
//         /// This method can be used to transform a {@code Stream} of optional
//         /// elements to a {@code Stream} of present value elements:
//         ///     Stream<Optional<T>> os = ..
//         ///     Stream<T> s = os.flatMap(Optional::stream)
//         /// @return the optional value as a {@code Stream}
//         public IEnumerable<TValue> ToEnumerable()
//         {
//             return !IsPresent() ? Array.Empty<TValue>() : ImmutableList.Create(value!);
//         }
//
//         /// <summary>
//         /// If a value is present, returns the value, otherwise returns
//         /// {@code other}.
//         /// </summary>
//         /// @param other the value to be returned, if no value is present.
//         ///        May be {@code null}.
//         /// @return the value, if present, otherwise {@code other}
//         public TValue OrElse(TValue other)
//         {
//             return value != null ? value : other;
//         }
//
//         /// <summary>
//         /// If a value is present, returns the value, otherwise returns the result
//         /// produced by the supplying function.
//         /// </summary>
//         /// @param supplier the supplying function that produces a value to be returned
//         /// @return the value, if present, otherwise the result produced by the
//         ///         supplying function
//         /// @throws NullPointerException if no value is present and the supplying
//         ///         function is {@code null}
//         public TValue OrElseGet(Func<TValue> supplier)
//         {
//             return value != null ? value : supplier();
//         }
//
//         /// <summary>
//         /// If a value is present, returns the value, otherwise throws
//         /// {@code NoSuchElementException}.
//         /// </summary>
//         /// @return the non-{@code null} value described by this {@code Optional}
//         /// @throws NoSuchElementException if no value is present
//         /// @since 10
//         public TValue OrElseThrow()
//         {
//             if (value == null)
//             {
//                 throw new NullReferenceException("No value present");
//             }
//             return value;
//         }
//
//         /// <summary>
//         /// If a value is present, returns the value, otherwise throws an exception
//         /// produced by the exception supplying function.
//         /// @apiNote
//         /// A method reference to the exception constructor with an empty argument
//         /// list can be used as the supplier. For example
//         /// </summary>,
//         /// {@code IllegalStateException::new}
//         /// @param <X> Type of the exception to be thrown
//         /// @param exceptionSupplier the supplying function that produces an
//         ///        exception to be thrown
//         /// @return the value, if present
//         /// @throws X if no value is present
//         /// @throws NullPointerException if no value is present and the exception
//         ///          supplying function is {@code null}
//         public TValue OrElseThrow(Func<Exception> exceptionSupplier)
//         {
//             if (value != null)
//             {
//                 return value;
//             }
//             throw exceptionSupplier();
//         }
//
//         private bool Equals(Optional<TValue>? other)
//         {
//             return EqualityComparer<TValue>.Default.Equals(value!, other!.value!);
//         }
//
//         public override bool Equals(object? obj)
//         {
//             if (ReferenceEquals(null, obj)) return false;
//             if (ReferenceEquals(this, obj)) return true;
//             return obj.GetType() == GetType() && Equals((Optional<TValue>) obj);
//         }
//
//         public override int GetHashCode()
//         {
//             return EqualityComparer<TValue>.Default.GetHashCode(value!);
//         }
//
//         public override string ToString()
//         {
//             return $"{nameof(value)}: {value}";
//         }
//     }
//
// }



