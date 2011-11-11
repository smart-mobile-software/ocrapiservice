using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Diagnostics;

namespace Phone.Controls
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Equivalent of FindName, but works on the visual tree to go through templates, etc.
        /// </summary>
        /// <param name="root">The node to search from</param>
        /// <param name="name">The name to look for</param>
        /// <returns>The found node, or null if not found</returns>
        public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
        {
            FrameworkElement temp = root.FindName(name) as FrameworkElement;
            if (temp != null)
                return temp;

            foreach (FrameworkElement element in root.GetVisualChildren())
            {
                temp = element.FindName(name) as FrameworkElement;
                if (temp != null)
                    return temp;
            }

            return null;
        }

        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            return VisualTreeHelper.GetParent(node) as FrameworkElement;
        }

        public static FrameworkElement GetVisualChild(this FrameworkElement node, int index)
        {
            return VisualTreeHelper.GetChild(node, index) as FrameworkElement;
        }

        /// <summary>
        /// Gets all the visual children of the element
        /// </summary>
        /// <param name="root">The element to get children of</param>
        /// <returns>An enumerator of the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                yield return VisualTreeHelper.GetChild(root, i) as FrameworkElement;
        }

        /// <summary>
        /// Gets the ancestors of the element, up to the root
        /// </summary>
        /// <param name="node">The element to start from</param>
        /// <returns>An enumerator of the ancestors</returns>
        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            FrameworkElement parent = node.GetVisualParent();
            while (parent != null)
            {
                yield return parent;
                parent = parent.GetVisualParent();
            }
        }

        /// <summary>
        /// Returns the first descendent with the given type
        /// </summary>
        /// <typeparam name="T">The type to look for</typeparam>
        /// <param name="root">The node to search</param>
        /// <returns>The found node, or null</returns>
        [Obsolete("Use Linq instead")]
        public static T GetVisualChild<T>(this DependencyObject root) where T : class
        {
            return (root as FrameworkElement).GetVisualDescendents().FirstOrDefault(o => o is T) as T;
        }

        /// <summary>
        /// Gets the VisualStateGroup with the given name, looking up the visual tree
        /// </summary>
        /// <param name="root">Element to start from</param>
        /// <param name="groupName">Name of the group to look for</param>
        /// <returns>The group, if found</returns>
        public static VisualStateGroup GetAncestorOrSelfVisualStateGroup(this FrameworkElement root, string groupName)
        {
            IList groups = VisualStateManager.GetVisualStateGroups(root);
            foreach (object o in groups)
            {
                VisualStateGroup group = o as VisualStateGroup;
                if (group != null && group.Name == groupName)
                    return group;
            }

            foreach (FrameworkElement ancestor in root.GetVisualAncestors())
            {
                groups = VisualStateManager.GetVisualStateGroups(ancestor);
                foreach (object o in groups)
                {
                    VisualStateGroup group = o as VisualStateGroup;
                    if (group != null && group.Name == groupName)
                        return group;
                }
            }

            return null;
        }

        public static bool TestItemVisibility(FrameworkElement item, FrameworkElement viewport, Orientation orientation, bool wantVisible)
        {
            GeneralTransform transform = item.TransformToVisual(viewport);
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(item.ActualWidth, item.ActualHeight));

            double min, max, testMin, testMax;
            if (orientation == System.Windows.Controls.Orientation.Vertical)
            {
                min = topLeft.Y;
                max = bottomRight.Y;
                testMin = 0;
                testMax = Math.Min(viewport.ActualHeight, double.IsNaN(viewport.Height) ? double.PositiveInfinity : viewport.Height);
            }
            else
            {
                min = topLeft.X;
                max = bottomRight.X;
                testMin = 0;
                testMax = Math.Min(viewport.ActualWidth, double.IsNaN(viewport.Width) ? double.PositiveInfinity : viewport.Width);
            }

            bool result = wantVisible;

            if (min >= testMax || max <= testMin)
                result = !wantVisible;

            Debug.WriteLine(String.Format("Check visibility {0}-{1} inside {2}-{3}, want visible {4} / result {5}",
              min, max, testMin, testMax, wantVisible, result));

            return result;
        }

        /// <summary>
        /// Returns the items that are visible in a given container.
        /// </summary>
        /// <remarks>This function assumes that items are ordered top-to-bottom or left-to-right; if items are in random positions it won't work</remarks>
        /// <typeparam name="T">The type of items being tested</typeparam>
        /// <param name="items">The items being tested; typically the children of a StackPanel</param>
        /// <param name="viewport">The viewport to test visibility against; typically a ScrollViewer</param>
        /// <param name="orientation">Wether to check for vertical or horizontal visibility</param>
        /// <returns>The items that are (at least partially) visible</returns>
        public static IEnumerable<T> GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation) where T : FrameworkElement
        {
            // Skip over the non-visible items, then take the visible items
            var skippedOverBeforeItems = items.SkipWhile((item) => TestItemVisibility(item, viewport, orientation, false));
            var keepOnlyVisibleItems = skippedOverBeforeItems.TakeWhile((item) => TestItemVisibility(item, viewport, orientation, true));
            return keepOnlyVisibleItems;
        }

        [Obsolete("Use Linq instead")]
        public static T GetAncestor<T>(this FrameworkElement root) where T : class
        {
            return root.GetVisualAncestors().FirstOrDefault(o => o is T) as T;
        }


        /// <summary>
        /// Performs a breadth-first enumeration of all the descendents in the tree
        /// </summary>
        /// <param name="root">The root node</param>
        /// <returns>An enumerator of all the children</returns>
        public static IEnumerable<FrameworkElement> GetVisualDescendents(this FrameworkElement root)
        {
            Queue<IEnumerable<FrameworkElement>> toDo = new Queue<IEnumerable<FrameworkElement>>();

            toDo.Enqueue(root.GetVisualChildren());
            while (toDo.Count > 0)
            {
                IEnumerable<FrameworkElement> children = toDo.Dequeue();
                foreach (FrameworkElement child in children)
                {
                    yield return child;
                    toDo.Enqueue(child.GetVisualChildren());
                }
            }
        }

        /// <summary>
        /// Returns all the descendents of a particular type
        /// </summary>
        /// <typeparam name="T">The type to look fork</typeparam>
        /// <param name="root">The root element</param>
        /// <param name="allAtSameLevel">Whether to stop searching the tree when the first item is found</param>
        /// <returns>List of the element found</returns>
        public static IEnumerable<T> GetVisualDescendents<T>(this FrameworkElement root, bool allAtSameLevel) where T : FrameworkElement
        {
            bool found = false;
            foreach (FrameworkElement e in root.GetVisualDescendents())
            {
                if (e is T)
                {
                    found = true;
                    yield return e as T;
                }
                else
                {
                    if (found == true && allAtSameLevel == true)
                        yield break;
                }
            }
        }

        /// <summary>
        /// Provides a depth-first search of leaf nodes in the visual tree
        /// </summary>
        /// <param name="root">The root object</param>
        /// <returns>All the leaf nodes</returns>
        public static IList<FrameworkElement> GetLeafNodes(this FrameworkElement root)
        {
            List<FrameworkElement> list = new List<FrameworkElement>();
            if (root == null)
                return null;

            int childCount = VisualTreeHelper.GetChildrenCount(root);
            if (childCount == 0)
            {
                list.Add(root);
            }
            else
            {
                for (int i = 0; i < childCount; i++)
                {
                    IList<FrameworkElement> res = GetLeafNodes(root.GetVisualChild(i));
                    if (res != null)
                        list.AddRange(res);
                }
            }
            return list;
        }

        public static string PrintChildTreeInfo(this FrameworkElement root)
        {
            List<string> results = new List<string>();
            root.GetChildTree("", "  ", results);
            StringBuilder sb = new StringBuilder();
            foreach (string s in results)
                sb.AppendLine(s);

            return sb.ToString();
        }

        private static void GetChildTree(this FrameworkElement root, string prefix, string addPrefix, List<string> results)
        {
            string thisElement = "";
            if (String.IsNullOrEmpty(root.Name))
                thisElement = "[Anon]";
            else
                thisElement = root.Name;

            thisElement += " " + root.GetType().Name;

            results.Add(prefix + thisElement);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
                child.GetChildTree(prefix + addPrefix, addPrefix, results);
            }
        }


        public static string PrintAncestorVisualTree(this FrameworkElement node)
        {
            List<string> tree = new List<string>();
            node.GetAncestorVisualTree(tree);
            StringBuilder sb = new StringBuilder();
            string prefix = "";
            foreach (string s in tree)
            {
                sb.AppendLine(prefix + s);
                prefix = prefix + "  ";
            }

            return sb.ToString();
        }

        private static void GetAncestorVisualTree(this FrameworkElement node, List<string> children)
        {
            string name = String.IsNullOrEmpty(node.Name) ? "[Anon]" : node.Name;
            string thisNode = name + ": " + node.GetType().Name;
            children.Insert(0, thisNode);
            FrameworkElement parent = node.GetVisualParent();
            if (parent != null)
                GetAncestorVisualTree(parent, children);
        }

        /// <summary>
        /// Returns a render transform of the specified type from the element, creating it if necessary
        /// </summary>
        /// <typeparam name="T">The type of transform (Rotate, Translate, etc)</typeparam>
        /// <param name="element">The element to check</param>
        /// <param name="create">Whether to create the transform if it isn't found</param>
        /// <returns>The specified transform, or null if not found and not created</returns>
        [Obsolete("Use the version with an enum instead")]
        public static RequestedTransform GetTransform<RequestedTransform>(this UIElement element, bool create) where RequestedTransform : Transform, new()
        {
            TransformCreationMode mode = TransformCreationMode.None;
            if (create)
                mode = TransformCreationMode.Create | TransformCreationMode.AddToGroup;

            return element.GetTransform<RequestedTransform>(mode);
        }
        /// <summary>
        /// Returns a render transform of the specified type from the element, creating it if necessary
        /// </summary>
        /// <typeparam name="T">The type of transform (Rotate, Translate, etc)</typeparam>
        /// <param name="element">The element to check</param>
        /// <param name="create">Whether to create the transform if it isn't found</param>
        /// <returns>The specified transform, or null if not found and not created</returns>
        [Obsolete("Don't use this - just add transforms to a visuale child")]
        public static RequestedTransform GetTransform<RequestedTransform>(this UIElement element, TransformCreationMode mode) where RequestedTransform : Transform, new()
        {
            Transform originalTransform = element.RenderTransform;
            RequestedTransform requestedTransform = null;
            MatrixTransform matrixTransform = null;
            TransformGroup transformGroup = null;

            // Current transform is null -- create if necessary and return
            if (originalTransform == null)
            {
                if ((mode & TransformCreationMode.Create) == TransformCreationMode.Create)
                {
                    requestedTransform = new RequestedTransform();
                    element.RenderTransform = requestedTransform;
                    return requestedTransform;
                }

                return null;
            }

            // Transform is exactly what we want -- return it
            requestedTransform = originalTransform as RequestedTransform;
            if (requestedTransform != null)
                return requestedTransform;


            // The existing transform is matrix transform - overwrite if necessary and return
            matrixTransform = originalTransform as MatrixTransform;
            if (matrixTransform != null)
            {
                if (matrixTransform.Matrix.IsIdentity
                  && (mode & TransformCreationMode.Create) == TransformCreationMode.Create
                  && (mode & TransformCreationMode.IgnoreIdentityMatrix) == TransformCreationMode.IgnoreIdentityMatrix)
                {
                    requestedTransform = new RequestedTransform();
                    element.RenderTransform = requestedTransform;
                    return requestedTransform;
                }

                return null;
            }

            // Transform is actually a group -- check for the requested type
            transformGroup = originalTransform as TransformGroup;
            if (transformGroup != null)
            {
                foreach (Transform child in transformGroup.Children)
                {
                    // Child is the right type -- return it
                    if (child is RequestedTransform)
                        return child as RequestedTransform;
                }

                // Right type was not found, but we are OK to add it
                if ((mode & TransformCreationMode.AddToGroup) == TransformCreationMode.AddToGroup)
                {
                    requestedTransform = new RequestedTransform();
                    transformGroup.Children.Add(requestedTransform);
                    return requestedTransform;
                }

                return null;
            }

            // Current ransform is not a group and is not what we want;
            // create a new group containing the existing transform and the new one
            if ((mode & TransformCreationMode.CombineIntoGroup) == TransformCreationMode.CombineIntoGroup)
            {
                transformGroup = new TransformGroup();
                transformGroup.Children.Add(originalTransform);
                transformGroup.Children.Add(requestedTransform);
                element.RenderTransform = transformGroup;
                return requestedTransform;
            }

            Debug.Assert(false, "Shouldn't get here");
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="subProperty"></param>
        /// <returns></returns>
        public static string GetTransformPropertyPath<RequestedType>(this FrameworkElement element, string subProperty) where RequestedType : Transform
        {
            Transform t = element.RenderTransform;
            if (t is RequestedType)
                return String.Format("(RenderTransform).({0}.{1})", typeof(RequestedType).Name, subProperty);

            else if (t is TransformGroup)
            {
                TransformGroup g = t as TransformGroup;
                for (int i = 0; i < g.Children.Count; i++)
                {
                    if (g.Children[i] is RequestedType)
                        return String.Format("(RenderTransform).(TransformGroup.Children)[" + i + "].({0}.{1})", typeof(RequestedType).Name, subProperty);
                }
            }

            return "";
        }

        /// <summary>
        /// Returns a plane projection, creating it if necessary
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="create">Whether or not to create the projection if it doesn't already exist</param>
        /// <returns>The plane project, or null if not found / created</returns>
        public static PlaneProjection GetPlaneProjection(this UIElement element, bool create)
        {
            Projection originalProjection = element.Projection;
            PlaneProjection projection = null;

            // Projection is already a plane projection; return it
            if (originalProjection is PlaneProjection)
                return originalProjection as PlaneProjection;

            // Projection is null; create it if necessary
            if (originalProjection == null)
            {
                if (create)
                {
                    projection = new PlaneProjection();
                    element.Projection = projection;
                }
            }

            // Note that if the project is a Matrix projection, it will not be
            // changed and null will be returned.
            return projection;
        }

        public static double GetVerticalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            return viewer.VerticalOffset;
        }

        public static double GetHorizontalScrollOffset(this ListBox list)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            return viewer.HorizontalOffset;
        }

        public static void SetVerticalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToVerticalOffset(offset);
                return;
            }

            // List is probably not loaded yet; defer scroll until loaded has fired
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                list.Loaded -= loadedHandler;
                viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
                if (viewer == null)
                    return;

                viewer.ScrollToVerticalOffset(offset);
            };

            list.Loaded += loadedHandler;
        }

        public static void SetHorizontalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
            if (viewer != null)
            {
                viewer.ScrollToHorizontalOffset(offset);
                return;
            }

            // List is probably not loaded yet; defer scroll until loaded has fired
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                list.Loaded -= loadedHandler;
                viewer = list.FindVisualChild("ScrollViewer") as ScrollViewer;
                if (viewer == null)
                    return;

                viewer.ScrollToHorizontalOffset(offset);
            };

            list.Loaded += loadedHandler;
        }
    }

    [Flags]
    public enum TransformCreationMode
    {
        /// <summary>
        /// Don't try and create a transform if it doesn't already exist
        /// </summary>
        None,

        /// <summary>
        /// Create a transform if none exists
        /// </summary>
        Create,

        /// <summary>
        /// Create and add to an existing group
        /// </summary>
        AddToGroup,

        /// <summary>
        /// Create a group and combine with existing transform; may break existing animations
        /// </summary>
        CombineIntoGroup,

        /// <summary>
        /// Treat identity matrix as if it wasn't there; may break existing animations
        /// </summary>
        IgnoreIdentityMatrix,

        /// <summary>
        /// Default mode; create a new transform or add to group
        /// </summary>
        CreateOrAddAndIgnoreMatrix = Create | AddToGroup | IgnoreIdentityMatrix,
    }
}
