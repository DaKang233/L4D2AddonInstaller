class DirectoryNode:
    """简易AST节点：表示目录/文件项"""
    def __init__(self, level, node_type, name):
        self.level = level  # 缩进层级（数字越小越顶层）
        self.node_type = node_type  # 'dir'（目录）/'file'（文件）
        self.name = name  # 节点名称（如bin、Debug、test.dll）
        self.parent = None  # 父节点
        self.children = []  # 子节点列表

    def add_child(self, child_node):
        """添加子节点"""
        child_node.parent = self
        self.children.append(child_node)

    def remove_self(self):
        """从父节点中移除自身（删除节点）"""
        if self.parent and self in self.parent.children:
            self.parent.children.remove(self)


def parse_directory_tree(text):
    """解析目录树文本为AST节点树"""
    lines = [line.rstrip('\r\n') for line in text.split('\n') if line.strip() != '']
    root = DirectoryNode(-1, 'root', 'root')  # 根节点（虚拟）
    current_parent = root

    for line in lines:
        # 1. 计算缩进层级（核心：统计行首的│/空格数量，判断层级）
        indent_chars = []
        for c in line:
            if c in ['│', ' ', '\t']:
                indent_chars.append(c)
            else:
                break
        indent_level = len(''.join(indent_chars)) // 4  # 每4个字符为1级（适配目录树缩进）
        line_content = line[len(indent_chars):].strip()

        # 2. 判断节点类型（目录/文件），提取名称
        if line_content.startswith(('├─', '└─')):
            # 目录项（├─xxx 或 └─xxx）
            node_name = line_content[2:].strip()
            node = DirectoryNode(indent_level, 'dir', node_name)
        else:
            # 文件项（无├─/└─前缀）
            node_name = line_content.strip()
            node = DirectoryNode(indent_level, 'file', node_name)

        # 3. 找到当前节点的父节点（层级匹配）
        while current_parent.level >= indent_level:
            current_parent = current_parent.parent
        current_parent.add_child(node)
        current_parent = node  # 更新当前父节点为新节点（供子节点挂载）

    return root


def clean_bin_obj_nodes(node):
    """递归清理所有bin/obj目录节点"""
    # 先递归清理子节点（深度优先）
    for child in list(node.children):  # 用list避免遍历中修改列表
        clean_bin_obj_nodes(child)
    
    # 若当前节点是bin/obj目录，删除自身
    if node.node_type == 'dir' and node.name.lower() in ['bin', 'obj']:
        node.remove_self()


def node_tree_to_text(node, prefix='', is_last=True):
    """将清理后的AST节点树还原为目录树文本"""
    lines = []
    # 跳过虚拟根节点
    if node.level == -1:
        for i, child in enumerate(node.children):
            lines.extend(node_tree_to_text(child, '', i == len(node.children)-1))
        return lines
    
    # 生成当前节点的文本行
    if node.node_type == 'dir':
        # 目录项：添加├─/└─前缀
        line_prefix = '└─' if is_last else '├─'
        current_line = f"{prefix}{line_prefix}{node.name}"
    else:
        # 文件项：仅保留缩进
        current_line = f"{prefix}  {node.name}"
    
    lines.append(current_line)

    # 处理子节点的缩进前缀
    child_prefix = prefix + ('    ' if is_last else '│   ')
    for i, child in enumerate(node.children):
        lines.extend(node_tree_to_text(child, child_prefix, i == len(node.children)-1))
    
    return lines


# ---------------------- 主流程 ----------------------
if __name__ == "__main__":
    # 1. 读取目录树文本（替换为你的文件路径）
    with open("directory_tree.txt", "r", encoding="utf-8") as f:
        original_text = f.read()

    # 2. 解析为AST节点树
    root_node = parse_directory_tree(original_text)

    # 3. 清理bin/obj节点
    clean_bin_obj_nodes(root_node)

    # 4. 还原为目录树文本
    cleaned_lines = node_tree_to_text(root_node)
    cleaned_text = '\n'.join(cleaned_lines)

    # 5. 保存结果
    with open("cleaned_tree_ast.txt", "w", encoding="utf-8") as f:
        f.write(cleaned_text)

    print("AST解析+清理完成！结果已保存到 cleaned_tree_ast.txt")
    print("\n清理后的目录树预览：")
    print(cleaned_text[:500])  # 打印前500字符预览