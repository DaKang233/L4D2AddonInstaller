import os
import fnmatch

class CustomDirectoryTreeGenerator:
    """自定义目录树生成器：支持排除指定目录/文件、指定排除层级"""
    def __init__(self, root_dir, exclude_rules=None, whitelist_rules=None, ignore_case=True):
        """
        初始化生成器
        :param root_dir: 目标根目录（如 E:/project）
        :param exclude_rules: 排除规则列表，格式：
               [{"name": "bin", "levels": [1,2,3]},  # 排除1-3级的bin目录
                {"name": "obj", "levels": "all"},    # 排除所有层级的obj目录
                {"name": "*.log", "levels": "all"}]  # 排除所有层级的.log文件
        :param whitelist_rules: 白名单规则列表，格式同exclude_rules，满足白名单规则的项不会被排除
        :param ignore_case: 是否忽略大小写（默认True）
        """
        self.root_dir = os.path.abspath(root_dir)
        self.exclude_rules = exclude_rules or []
        self.whitelist_rules = whitelist_rules or []
        self.ignore_case = ignore_case
        self.tree_lines = []  # 存储目录树文本行

    def _is_excluded(self, name, level):
        """判断当前项是否需要排除"""
        # 统一大小写（如果开启忽略）
        check_name = name.lower() if self.ignore_case else name
        
        for rule in self.exclude_rules:
            rule_name = rule["name"].lower() if self.ignore_case else rule["name"]
            rule_levels = rule["levels"]
            
            # 匹配名称（支持通配符，如*.log）
            if fnmatch.fnmatch(check_name, rule_name):
                # 匹配层级
                if rule_levels == "all" or level in rule_levels:
                    return True
        return False
    
    def _is_whitelisted(self, name, level):
        check_name = name.lower() if self.ignore_case else name

        for rule in self.whitelist_rules:
            rule_name = rule["name"].lower() if self.ignore_case else rule["name"]
            rule_levels = rule["levels"]

            if fnmatch.fnmatch(check_name, rule_name):
                if rule_levels == "all" or level in rule_levels:
                    return True
        return False
    
    def _append_item(self, item_path, item, dirs, files):
        """添加项到目录树"""
        if os.path.isdir(item_path):
            dirs.append(item)
        else:
            files.append(item)

    def _generate_tree(self, current_dir, level=0, prefix="", is_last=False):
        """
        递归生成目录树（核心逻辑）
        :param current_dir: 当前遍历的目录
        :param level: 当前层级（根目录为0级）
        :param prefix: 缩进前缀（控制├─/└─/│的显示）
        :param is_last: 是否是父目录的最后一个子项
        """
        # 获取当前目录下的所有项（按目录在前、文件在后排序）
        try:
            items = os.listdir(current_dir)
        except PermissionError:
            # 处理权限不足的情况
            self.tree_lines.append(f"{prefix}└─[权限不足，无法访问]")
            return
        except Exception as e:
            self.tree_lines.append(f"{prefix}└─[访问错误：{str(e)}]")
            return

        # 区分目录和文件，分别排序
        dirs = []
        files = []
        for item in items:
            item_path = os.path.join(current_dir, item)
            # 白名单优先
            if self._is_whitelisted(item, level + 1):
                self._append_item(item_path, item, dirs, files)
                continue

            # 再判断排除
            if self._is_excluded(item, level + 1):
                continue

            # 默认加入
            self._append_item(item_path, item, dirs, files)
        
        # 合并：目录在前，文件在后，按名称排序
        sorted_items = sorted(dirs) + sorted(files)
        total_items = len(sorted_items)

        for idx, item in enumerate(sorted_items):
            item_is_last = idx == total_items - 1
            item_path = os.path.join(current_dir, item)
            item_level = level + 1

            # 生成当前项的前缀
            if level == 0:
                # 根目录的子项，无前置│
                current_prefix = "└─" if item_is_last else "├─"
            else:
                # 非根目录，根据父项是否是最后一个，决定前缀是│还是空格
                parent_prefix = "│   " if not is_last else "    "
                current_prefix = prefix + parent_prefix + ("└─" if item_is_last else "├─")
            
            # 添加当前项到目录树
            self.tree_lines.append(f"{current_prefix}{item}")

            # 如果是目录，递归遍历其子项
            if os.path.isdir(item_path):
                self._generate_tree(item_path, item_level, prefix, item_is_last)

    def generate(self):
        """生成最终的目录树文本"""
        # 清空历史结果
        self.tree_lines = []
        # 添加根目录标题
        self.tree_lines.append(f"{self.root_dir}")
        # 递归生成子项
        self._generate_tree(self.root_dir)
        # 合并为最终文本
        return "\n".join(self.tree_lines)

# ---------------------- 用户交互与使用 ----------------------
def main():
    print("===== 自定义目录树生成器 =====")
    # 1. 获取用户输入的目标目录
    while True:
        root_dir = input("\n请输入要生成目录树的根目录（如 E:/project）：").strip()
        root_dir = os.path.abspath(root_dir)  # 转换为绝对路径
        if os.path.isdir(root_dir):
            break
        print(f"错误：目录 '{root_dir}' 不存在，请重新输入！")

    # 2.1 配置排除规则（新手可直接修改这里的默认规则）
    print("\n【默认排除规则】：排除所有层级的bin/obj目录、.git目录、.log文件")
    custom_exclude = input("是否自定义排除规则？(y/n，默认n)：").strip().lower()
    if custom_exclude == "y":
        exclude_rules = []
        print("\n请输入排除规则（输入空行结束）：")
        print("规则格式示例：")
        print("  输入 'bin:all' → 排除所有层级的bin目录")
        print("  输入 'obj:1,2' → 排除1-2级的obj目录")
        print("  输入 '*.log:all' → 排除所有层级的.log文件")
        while True:
            rule_input = input("请输入排除规则：").strip()
            if not rule_input:
                break
            # 解析用户输入的规则
            if ":" not in rule_input:
                print("格式错误！请按 '名称:层级' 格式输入（如 bin:all）")
                continue
            name, levels_str = rule_input.split(":", 1)
            # 解析层级
            if levels_str.lower() == "all":
                levels = "all"
            else:
                try:
                    levels = [int(level.strip()) for level in levels_str.split(",")]
                except ValueError:
                    print("层级格式错误！请输入数字（如 1,2）或 all")
                    continue
            exclude_rules.append({"name": name, "levels": levels})
    else:
        # 默认排除规则（可根据需求修改）
        exclude_rules = [
            {"name": "bin", "levels": "all"},       # 排除所有层级的bin目录
            {"name": "obj", "levels": "all"},       # 排除所有层级的obj目录
            {"name": ".git", "levels": "all"},      # 排除所有层级的.git目录
            {"name": "*.log", "levels": "all"},     # 排除所有层级的.log文件
            {"name": "*.pdb", "levels": "all"},      # 排除所有层级的.pdb文件
            {"name": "packages", "levels": "all"},   # 排除所有层级的packages目录
            {"name": ".vs", "levels": "all"},
            {"name": ".git", "levels": "all"},
        ]

    # 2.2 配置白名单（可使用正则表达式）(此时忽略排除规则，排除规则失效)
    print("\n【白名单】：如果某些目录/文件满足白名单规则，则不会被排除")
    custom_whitelist = input("是否自定义白名单规则？(y/n，默认n)：").strip().lower()
    if custom_whitelist == "y":
        whitelist_rules = []
        print("\n请输入白名单规则（输入空行结束）：")
        while True:
            rule_input = input("请输入白名单规则：").strip()
            if not rule_input:
                break
            # 解析用户输入的规则
            if ":" not in rule_input:
                print("格式错误！请按 '名称:层级' 格式输入（如 bin:all）")
                continue
            name, levels_str = rule_input.split(":", 1)
            # 解析层级
            if levels_str.lower() == "all":
                levels = "all"
            else:
                try:
                    levels = [int(level.strip()) for level in levels_str.split(",")]
                except ValueError:
                    print("层级格式错误！请输入数字（如 1,2）或 all")
                    continue
            whitelist_rules.append({"name": name, "levels": levels})
    else:
        # 默认白名单规则（可根据需求修改）
        whitelist_rules = []

    # 3. 生成目录树
    generator = CustomDirectoryTreeGenerator(root_dir, exclude_rules, whitelist_rules)
    tree_text = generator.generate()

    # 4. 输出结果（可选保存到文件）
    print("\n===== 生成的目录树 =====")
    print(tree_text)

    # 保存到文件
    save_file = input("\n是否将目录树保存到文件？(y/n，默认n)：").strip().lower()
    if save_file == "y":
        while True:
            input_path = input("请输入完整保存路径（默认保存到根目录；输入空行则使用默认路径）：").strip()
            if input_path:
                try:
                    save_path = os.path.abspath(input_path)
                except Exception as e:
                    print(f"错误：无法解析保存路径 '{input_path}'，请重新输入！")
                    continue
            else:
                save_path = os.path.join(root_dir, "custom_directory_tree.txt")
                break
        with open(save_path, "w", encoding="utf-8") as f:
            f.write(tree_text)
        print(f"目录树已保存到：{save_path}")

if __name__ == "__main__":
    main()